using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkIFCModelStacker : IFCModelStacker
{
    [ServerRpc(RequireOwnership = false)]
    public void SpawnNetworkObjectServerRpc(  GameObject root)
    {
        InitializeIFCModel( root );
        _ifcRoot = root;
        _xrGrabableObjectInstance = Instantiate( _xrGrabableObject, _modelTransform );
        
        NetworkObject spawnedNetworkObject = _xrGrabableObjectInstance.GetComponent < NetworkObject >();
        spawnedNetworkObject.Spawn( true );
        
        _ifcRoot.transform.SetParent( _xrGrabableObjectInstance.transform );

        ResetInstanceTransform();
        AddInteractableComponentsToModel();
        AddInteractableComponentsToChilds();    
    }

}
