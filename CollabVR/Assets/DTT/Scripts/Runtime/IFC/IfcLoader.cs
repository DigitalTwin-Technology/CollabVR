using System;
using System.Collections;
using IFC;
using UnityEngine;
using UnityEngine.Networking;

public class IfcLoader : MonoBehaviour
{
    private const string _url = "https://ashvin-integration.digitaltwin.technology/Models/mile_center-model-geometry_pivot_origin.glb";

    private readonly GlbLoader _glbLoader = new();

    public void LoadIfc(int assetId, int fileId, Action<GameObject> onIfcLoaded)
    {
        StartCoroutine(LoadIfc(_url, onIfcLoaded));
    }

    private IEnumerator LoadIfc(string endpoint, Action<GameObject> onIfcLoaded)
    {
        using UnityWebRequest request = UnityWebRequest.Get(endpoint);
        
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to get {endpoint}. Error: {request.error}");
            onIfcLoaded?.Invoke(null);
            yield break;
        }

        _glbLoader.Load(request.downloadHandler.data, onIfcLoaded);
    }
}