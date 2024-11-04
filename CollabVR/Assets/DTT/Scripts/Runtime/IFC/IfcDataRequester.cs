using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace IFC
{
    public class IfcDataRequester : MonoBehaviour
    {
        private const string _url = "https://server.ashvin-integration.digitaltwin.technology/neo4j";

        public void RequestData(string ifcObjectId, Action<Dictionary<string, string>> onRequestedData)
        {
            StartCoroutine(RequestDataCoroutine(ifcObjectId, onRequestedData));
        }

        private IEnumerator RequestDataCoroutine(string ifcObjectId, Action<Dictionary<string, string>> onRequestedData)
        {
            using var request = GetQueryRequest(ifcObjectId);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                onRequestedData?.Invoke(null);
                yield break;
            }

            try
            {
                var jobject = JObject.Parse(request.downloadHandler.text);

                var json = (JObject)(jobject["results"][0]["data"][0]["row"][0]);
                var dict = new Dictionary<string, string>();

                foreach (var (key, value) in json) dict.Add(key, value.ToString());

                onRequestedData?.Invoke(dict);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                onRequestedData?.Invoke(null);
            }
        }

        public UnityWebRequest GetQueryRequest(string ifcObjectId)
        {
            var request = new UnityWebRequest(_url);
            request.method = "POST";
            request.SetRequestHeader("Authorization", GetAuthorization("neo4j", "password"));
            request.SetRequestHeader("Accept", "application/json;charset=UTF-8");
            request.SetRequestHeader("Content-Type", "application/json");

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(
                $"{{\"statements\": [{{\"statement\": \"MATCH (n {{GlobalId: '{ifcObjectId}'}}) RETURN PROPERTIES(n)\"}}]}}"));

            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }
        
        private static string GetAuthorization( string username, string password )
        {
            return "Basic " + Convert.ToBase64String( Encoding.UTF8.GetBytes( $"{username}:{password}" ) );
        }
    }
}