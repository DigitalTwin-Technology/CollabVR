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
    private IFCModelStacker _ifcModelStacker;

    public IFCModelStacker IfcModelStacker => _ifcModelStacker;
    
    [SerializeField]
    private IFCModelInteractor _ifcModelInteractor;

    private void Awake()
    {
        _ifcLoaderTest.OnLoadModel += _ifcModelStacker.InitializeIFCModel;
    }

    
    
    public void ResetInstanceTransformFromScene()
    {
        _ifcModelStacker.ResetInstanceTransform( );
        _ifcModelInteractor.SetMode( IFCModelInteractor.IFCMode.INTERACTABLE );
        
    }
    
    public void SetActiveInstance( bool value )
    {
        if( _ifcModelStacker.XRGrabbableObjectInstance != null)
        {
            _ifcModelStacker.XRGrabbableObjectInstance.SetActive( value );
            Debug.Log( "SetActiveInstance Correct" );
            //StartCoroutine( _ifcModelStacker.SetParentIFC(_ifcModelStacker.XRGrabbableObjectInstance) );
        }
    }
    
    
    
    
    private void OnDestroy()
    {
        _ifcLoaderTest.OnLoadModel -=  _ifcModelStacker.InitializeIFCModel;
    }

    

}
