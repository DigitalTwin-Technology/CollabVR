using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guid = BimViz.Guid;
using UnityEngine.XR.Interaction.Toolkit;

namespace IFC.Test
{

public class IFCModelInteractor : MonoBehaviour
{
    public enum IFCMode
    {
        INTERACTABLE,
        CHILD_INTERACTABLE,
        NON_INTERACTABLE,
        
    }
    
    [SerializeField]
    private IfcDataRequester _ifcDataRequester;
    
    [SerializeField] 
    private DisplayDataTest _displayDataTest;
    
    [SerializeField]
    private IFCModelController _ifcModelController;

    [SerializeField]
    private Color _colorOnHover;
    
    [SerializeField]
    private Color _colorOnClick;
    
    [SerializeField]
    private Color _colorDefault;
    
    private const string TagIfc = "IFC";
   
    private bool _interactableChildsIFC;
    
    private IFCMode IfcMode = IFCMode.INTERACTABLE;

    
    public void OnHoverEntered( HoverEnterEventArgs args )
    {
        Transform selectedTransform = args.interactableObject.transform;

        if( selectedTransform.CompareTag( TagIfc ) && _interactableChildsIFC )
        {
            // Set Color
            Material ifcMat = selectedTransform.GetComponent < Renderer >().material;
            ifcMat.color = _colorOnHover;
        }
    }
    
    public void OnHoverExited( HoverExitEventArgs args )
    {
        Transform selectedTransform = args.interactableObject.transform;

        if( selectedTransform.CompareTag( TagIfc ) && _interactableChildsIFC )
        {
            // Set Color
            Material ifcMat = selectedTransform.GetComponent < Renderer >().material;
            ifcMat.color = _colorDefault;
        }
    }
    
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"{args.interactorObject} Enter over {args.interactableObject}", this);

        Transform selectedTransform = args.interactableObject.transform;

        if( selectedTransform.CompareTag( TagIfc ) && _interactableChildsIFC)
        {
            // Set RigidBody
            SetActiveRigidbody( selectedTransform, true );

            // Set Color
            Material ifcMat = selectedTransform.GetComponent < Renderer >().material;
            ifcMat.color = _colorOnClick;
            
            //Set Rotation
            // selectedTransform.rotation = Quaternion.Euler(
            //     selectedTransform.rotation.x - 89,
            //     selectedTransform.rotation.y,
            //     selectedTransform.rotation.z );
            
            // Request Data
            _ifcDataRequester.RequestData( Guid.GetLongId( selectedTransform.name ), _displayDataTest.DisplayData );
        }

    }

    public void OnSelectExited( SelectExitEventArgs args )
    {
        Debug.Log($"{args.interactorObject} Exit over {args.interactableObject}", this);
        
        Transform selectedTransform = args.interactableObject.transform;

        if( selectedTransform.CompareTag( TagIfc ) && _interactableChildsIFC)
        {
            // Set RigidBody
            SetActiveRigidbody( selectedTransform, false );
            
            // Set Color
            Material ifcMat = selectedTransform.GetComponent < Renderer >().material;
            ifcMat.color = _colorDefault;
            
            //Set Rotation
            // selectedTransform.rotation = Quaternion.Euler(
            //     selectedTransform.rotation.x - 89,
            //     selectedTransform.rotation.y,
            //     selectedTransform.rotation.z );
        }
    }

    public void OnToggleChildSelectionChanged( bool value )
    {
        if( value )
            SetModeChildInteractable();
        else
            SetModeInteractable();
    }

    public void SetModeChildInteractable()
    {
        SetMode( IFCMode.CHILD_INTERACTABLE );
    }
    public void SetModeInteractable()
    {
        SetMode( IFCMode.INTERACTABLE );
    }
    
    public IFCMode SetMode (IFCMode mode)
    {
        IfcMode = mode;
        
        switch( mode )
        {
            case IFCMode.INTERACTABLE:
                _interactableChildsIFC = false;
                _ifcModelController.IfcModelStacker.XRGrabbableObjectInstance.transform.GetComponent < BoxCollider >().enabled = true;
                
                foreach( Transform child in _ifcModelController.IfcModelStacker.IFCRoot.transform.GetChild( 0 ) )
                {
                    SetActiveCollider( child, false );
                    SetActiveRigidbody( child, true );
                    child.localPosition = Vector3.zero;
                    child.rotation = Quaternion.Euler( -90, -90, 0);
                    Material ifcMat = child.GetComponent < Renderer >().material;
                    ifcMat.color = _colorDefault;
                }
                break;
            
            case IFCMode.CHILD_INTERACTABLE:
                _interactableChildsIFC = true;
                _ifcModelController.IfcModelStacker.XRGrabbableObjectInstance.transform.GetComponent < BoxCollider >().enabled = false;
                
                foreach( Transform child in _ifcModelController.IfcModelStacker.IFCRoot.transform.GetChild( 0 ) )
                {
                    SetActiveCollider( child, true );
                }
                // Activate Sphere Model Transformer, in order to move Root Model if needed 
                break;
            
            case IFCMode.NON_INTERACTABLE:
                _interactableChildsIFC = false;
                _ifcModelController.IfcModelStacker.XRGrabbableObjectInstance.transform.GetComponent < BoxCollider >().enabled = false;

                foreach( Transform child in _ifcModelController.IfcModelStacker.IFCRoot.transform.GetChild( 0 ) )
                {
                    SetActiveCollider( child, false );
                    child.localPosition = Vector3.zero;
                }
                break;
        }

        return mode;
    }
    
    public void SetActiveCollider(Transform obj, bool value)
    {
        obj.GetComponent < MeshCollider >().enabled = value;
    }
    
    public void SetActiveRigidbody(Transform obj, bool value)
    {
        obj.GetComponent < Rigidbody >().isKinematic = value;
    }
    


    
    
}

}

