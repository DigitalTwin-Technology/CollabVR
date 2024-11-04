using System;
using System.Collections;
using System.Collections.Generic;
using IFC.Test;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class IFCModelController : MonoBehaviour
{

    [SerializeField]
    private IfcLoaderTest _ifcLoaderTest;
    
    [SerializeField]
    private NetworkIFCModelStacker _networkIfcModelStacker;

    public IFCModelStacker IfcModelStacker => _networkIfcModelStacker;
    
    [SerializeField]
    private IFCModelInteractor _ifcModelInteractor;

    private void Awake()
    {
        //_ifcLoaderTest.OnLoadModel += _networkIfcModelStacker.SpawnNetworkObjectServerRpc;
    }

    
    
    public void ResetInstanceTransformFromScene()
    {
        _networkIfcModelStacker.ResetInstanceTransform( );
        _ifcModelInteractor.SetMode( IFCModelInteractor.IFCMode.INTERACTABLE );
        
    }
    
    public void SetActiveInstance( bool value )
    {
        if( _networkIfcModelStacker.XRGrabbableObjectInstance != null)
        {
            _networkIfcModelStacker.XRGrabbableObjectInstance.SetActive( value );
            //StartCoroutine( _ifcModelStacker.SetParentIFC(_ifcModelStacker.XRGrabbableObjectInstance) );
        }
    }
    
    
    
    
    private void OnDestroy()
    {
        _ifcLoaderTest.OnLoadModel -=  _networkIfcModelStacker.InitializeIFCModel;
    }

    

}
