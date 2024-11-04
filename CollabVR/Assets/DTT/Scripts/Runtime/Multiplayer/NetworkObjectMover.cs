using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkObjectMover : NetworkBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Vector3 lastPosition;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void Start()
    {
        if (IsServer)
        {
            // Set the initial position
            lastPosition = transform.position;
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            // Server checks if the position has changed and updates clients
            if (transform.position != lastPosition)
            {
                lastPosition = transform.position;
                UpdateClientPositionClientRpc(transform.position);
            }
        }
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnGrabEnd);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnGrabEnd);
    }

    private void OnGrabEnd(SelectExitEventArgs args)
    {
        //if (IsOwner)
        //{
            // When grab ends, send position update to the server
            MoveObjectServerRpc(transform.position);
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    void MoveObjectServerRpc(Vector3 newPosition)
    {
        // Update server position
        transform.position = newPosition;

        // Broadcast new position to all clients
        UpdateClientPositionClientRpc(newPosition);
    }

    [ClientRpc]
    void UpdateClientPositionClientRpc(Vector3 newPosition)
    {
        // Update client position
        if (!IsOwner) // Avoid updating the owner client who is already handling the position
        {
            transform.position = newPosition;
        }
    }
}
