using System;
using GLTFast;
using UnityEngine;

namespace IFC
{
    public class GlbLoader
    {
        public async void Load(byte[] glbData, Action<GameObject> onLoadGlb)
        {
            var glb = new GltfImport();
            if (!await glb.LoadGltfBinary(glbData))
            {
                Debug.LogError("Error Loading GLB.");
                onLoadGlb?.Invoke(null);
                return;
            }

            var root = new GameObject("IfcModel");
            if (await glb.InstantiateMainSceneAsync(root.transform)) onLoadGlb?.Invoke(root);
            Debug.Log( "Load Correct" );

        }
    }
}