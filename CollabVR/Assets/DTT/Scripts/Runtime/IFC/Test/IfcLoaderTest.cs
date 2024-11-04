using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace IFC.Test
{
    public class IfcLoaderTest : MonoBehaviour
    {
        private const int AssetIdDemo6 = 34;
        private const int FileIdDemo6 = 36;

        [SerializeField] private IfcLoader _ifcLoader;
        
        private Action < GameObject >  _onLoadModel;

        public Action < GameObject > OnLoadModel
        {
            get => _onLoadModel;
            set => _onLoadModel = value;
        }
        
        public UnityEvent<GameObject> IfcLoadedEvent;

        private bool _modelLoaded;

        public void LoadDemo6()
        {
            if( !_modelLoaded )
                //LoadDemo6ServerRcp();
                _ifcLoader.LoadIfc(AssetIdDemo6, FileIdDemo6, OnIfcLoaded);
        }

        [ServerRpc(RequireOwnership = false)]
        public void LoadDemo6ServerRcp()
        {
            Debug.Log( "Trying to Load Demo 6" );
            if (!_modelLoaded)
                _ifcLoader.LoadIfc(AssetIdDemo6, FileIdDemo6, OnIfcLoaded);
        }

        private void OnIfcLoaded(GameObject root)
        {
            Debug.Log("Model Loaded.");
            _modelLoaded = root != null;

            if ( _modelLoaded )
            {
                _onLoadModel?.Invoke( root );
                IfcLoadedEvent?.Invoke( root );
                
            }
        }
    }
}