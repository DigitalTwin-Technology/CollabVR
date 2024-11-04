using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ashvin.Neo4j;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace DTT.Scripts.Runtime.Neo4j
{
    public class Neo4jManager : MonoBehaviour
    {
        [SerializeField] private string _serverURL;

        [SerializeField] private string _username;

        [SerializeField] private string _password;

        public static Neo4jManager Instance;

        private void Awake()
        {
            Instance = this;
        }

#if UNITY_EDITOR
        private void Start()
        {
            _username = "neo4j";
            _password = "password";
        }
#endif

        private static string GetAuthorization(string username, string password)
        {
            return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        }

        public void SendQuery(
            Statement statement,
            UnityAction<Neo4jResult> onSuccess = null,
            UnityAction<Neo4jResult> onError = null,
            bool log = false)
        {
            SendQuery(new[] { statement }, onSuccess, onError, log);
        }

        public void SendQuery(
            IEnumerable<Statement> statements,
            UnityAction<Neo4jResult> onSuccess = null,
            UnityAction<Neo4jResult> onError = null,
            bool log = false)
        {
            StartCoroutine(SendQueryCoroutine(statements, onSuccess, onError, log));
        }

        private IEnumerator SendQueryCoroutine(
            IEnumerable<Statement> statements,
            UnityAction<Neo4jResult> onSuccess = null,
            UnityAction<Neo4jResult> onError = null,
            bool log = false)
        {
            using var request = GetQueryRequest(statements);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Failed Node4j on database " +
                    ". Error: " +
                    request.error +
                    " Statement: " +
                    statements.First().statement);

                onError?.Invoke(
                    new Neo4jResult
                    {
                        Errors = new[]
                        {
                            new Neo4jResult.Error
                            {
                                Code = "Request Failed",
                                Message = "Failed Node4j " + " Request: " + request.error
                            }
                        }
                    });

                request.Dispose();

                yield break;
            }

            if (log) Debug.Log("Neo4j Result: " + request.downloadHandler.text);

            Neo4jResult neo4JResult;

            try
            {
                neo4JResult = ParseResult(request.downloadHandler.text);
            }
            catch (Exception error)
            {
                Debug.LogError("Failed Parsing Neo4jResult");

                neo4JResult = new Neo4jResult
                {
                    Errors = new[]
                    {
                        new Neo4jResult.Error
                        {
                            Code = "Error Parsing Neo4jResult. Error: " + error,
                            Message = request.downloadHandler.text
                        }
                    }
                };

                request.Dispose();

                onError?.Invoke(neo4JResult);

                yield break;
            }

            request.Dispose();

            onSuccess?.Invoke(neo4JResult);
        }

        public static Neo4jResult ParseResult(string jsonText)
        {
            var neo4JResult = new Neo4jResult();

            var jobject = JObject.Parse(jsonText);

            neo4JResult.Errors = jobject["errors"]?.Select(
                e => new Neo4jResult.Error
                {
                    Code = (string)e["code"], Message = (string)e["message"]
                }).ToArray();

            neo4JResult.Results = jobject["results"]?.Select(
                r => new Neo4jResult.Result
                {
                    Columns = r["columns"].ToObject<string[]>(),
                    Data = r["data"].Select(
                        d => new Neo4jResult.Data
                        {
                            Row = d["row"].Select(r => r.ToString()).ToArray(),
                            Meta = d["meta"].Select(m => m.ToString()).ToArray()
                        }).ToArray()
                }).ToArray();

            return neo4JResult;
        }

        public UnityWebRequest GetQueryRequest(IEnumerable<Statement> statements)
        {
            var url = _serverURL + "/neo4j";
            var request = new UnityWebRequest(url);
            request.method = "POST";

            //request.SetRequestHeader( "User-Auth", $"{m_Username.Value}:{m_Password.Value}" );
            request.SetRequestHeader("Authorization", GetAuthorization(_username, _password));
            request.SetRequestHeader("Accept", "application/json;charset=UTF-8");
            request.SetRequestHeader("Content-Type", "application/json");

            //request.SetRequestHeader( "access-mode", "READ" );

            //TODO:
            //request.SetRequestHeader( "X-Stream", "true" );

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(StatementsToString(statements)));

            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        private static string StatementsToString(IEnumerable<Statement> statements)
        {
            return "{\"statements\":" + JsonConvert.SerializeObject(statements) + '}';
        }
    }
}