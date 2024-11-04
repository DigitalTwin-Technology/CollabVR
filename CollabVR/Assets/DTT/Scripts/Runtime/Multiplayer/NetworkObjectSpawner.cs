using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkObjectSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject _CubeNetworkObject;

    private GameObject _spawnedObject;
    private NetworkObject _spawnedNetworkObject;

    public void SpawnNetworkObject()
    {
        SpawnNetworkObjectServerRpc(  );
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnNetworkObjectServerRpc(  )
    {
        _spawnedObject = Instantiate( _CubeNetworkObject );
        _spawnedNetworkObject = _spawnedObject.GetComponent < NetworkObject >();
        _spawnedNetworkObject.Spawn( true );
    }


    public override void OnDestroy()
    {
        if( _spawnedObject != null )
        {
            _spawnedObject.GetComponent<NetworkObject>().Despawn( true );
            Destroy( _spawnedObject );
        }
  
    }
}
