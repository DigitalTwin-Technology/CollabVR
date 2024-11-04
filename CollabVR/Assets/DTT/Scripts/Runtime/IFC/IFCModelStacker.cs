using System;
using System.Collections;
using System.Collections.Generic;
using BimViz;
using IFC.Test;
using Unity.VisualScripting;
using UnityEngine;


public class IFCModelStacker : MonoBehaviour
{
    [SerializeField]
    protected Transform _modelTransform;

    [SerializeField]
    private Vector3 _modelRotation;

    [SerializeField]
    [Range( 0f, 5f )]
    private float _modelScale = 1f;

    [SerializeField]
    private string _tagIFC;

    [SerializeField]
    private ModelBoundsController _modelBoundsController;

    [SerializeField]
    protected GameObject _xrGrabableObject;
    
    protected GameObject _xrGrabableObjectInstance;
    public GameObject XRGrabbableObjectInstance
    {
        get => _xrGrabableObjectInstance;
    }

    protected GameObject _ifcRoot;
    public GameObject IFCRoot
    {
        get => _ifcRoot;
    }
    
    public void InitializeIFCModel( GameObject root )
    {
        _ifcRoot = root;
        _xrGrabableObjectInstance = Instantiate( _xrGrabableObject, _modelTransform );
        _ifcRoot.transform.SetParent( _xrGrabableObjectInstance.transform );

        ResetInstanceTransform();
        AddInteractableComponentsToModel();
        AddInteractableComponentsToChilds();    
    }

    private void Update()
    {
       // if (_xrGrabableObjectInstance != null)
            //Debug.Log( " ---- UPDATE ---- XR Parent: " + _xrGrabableObjectInstance.transform.parent.name );
    }

    protected void AddInteractableComponentsToModel( )
    {
        GameObject sceneObj = _xrGrabableObjectInstance.transform.GetChild( 0 ).gameObject;
        Vector3 boundSize = _modelBoundsController.GetGlobalBoundSize(sceneObj);
        _modelBoundsController.OnModelLoaded(sceneObj);
        
        BoxCollider boxCollider = _xrGrabableObjectInstance.AddComponent < BoxCollider >();
        boxCollider.size = boundSize / _modelScale;

        StartCoroutine( RegisterXRGrabInteractableCoroutine( boxCollider ) );

    }

    protected void AddInteractableComponentsToChilds( )
    {

        GameObject sceneObj = _xrGrabableObjectInstance.transform.GetChild( 0 ).GetChild( 0 ).gameObject;
        
        foreach ( Transform child in sceneObj.transform )
        {
            MeshCollider meshCollider = child.AddComponent < MeshCollider >();
            meshCollider.convex = true;
            meshCollider.enabled = false;
            
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable xrGrab = child.AddComponent < UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable >();
            xrGrab.distanceCalculationMode = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.DistanceCalculationMode.TransformPosition;
            xrGrab.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple;
            xrGrab.useDynamicAttach = true;
            //.trackRotation = false;
            Rigidbody rb = child.GetComponent < Rigidbody >();
            rb.useGravity = false;
            rb.drag = 5;
            rb.angularDrag= 5;
            rb.isKinematic = true;
            
            child.tag = _tagIFC;
        }
    }
    
    public void ResetInstanceTransform( )
    {
        _xrGrabableObjectInstance.transform.SetParent( _modelTransform );

        _xrGrabableObjectInstance.transform.rotation = Quaternion.Euler( _modelRotation );
        _xrGrabableObjectInstance.transform.localScale = new Vector3( _modelScale, _modelScale, _modelScale );
        _xrGrabableObjectInstance.transform.localPosition = Vector3.zero;

        _ifcRoot.transform.localScale = Vector3.one;
        _ifcRoot.transform.localRotation = Quaternion.identity;
        _ifcRoot.transform.localPosition = Vector3.zero;

    }

    IEnumerator RegisterXRGrabInteractableCoroutine(BoxCollider boxCollider)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable xrGrabInteractable = _xrGrabableObjectInstance.GetComponent < UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable >();
        xrGrabInteractable.colliders.Add( boxCollider );
        xrGrabInteractable.interactionManager.UnregisterInteractable( xrGrabInteractable.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable>() );
        xrGrabInteractable.interactionManager.RegisterInteractable( xrGrabInteractable.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable>() );
        yield return 0;
        
        _xrGrabableObjectInstance.transform.SetParent( _modelTransform );
    }
}
