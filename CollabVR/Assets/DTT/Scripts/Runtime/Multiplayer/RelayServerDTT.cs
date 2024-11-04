using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayServerDTT : MonoBehaviour
{

    public TMP_Text m_TextJoinCode;
    public TMP_Text m_TextServerCreated;
    public TMP_Text m_InputTextJoinCode;
    public GameObject m_ButtonCreateServer;
    public GameObject m_ButtonJoinServer;
    public GameObject m_ButtonExitServer;
    
    private string roomID = "";
   private async void Start()
   {
       await UnityServices.InitializeAsync();

       AuthenticationService.Instance.SignedIn += () =>
       {
           Debug.Log( "Signed in " + AuthenticationService.Instance.PlayerId );

       };

       await AuthenticationService.Instance.SignInAnonymouslyAsync();

   }
   
   [ContextMenu("Create Relay")]
   public async void CreateRelay()
   {
       try
       {
           Allocation allocation = await RelayService.Instance.CreateAllocationAsync( maxConnections: 3 );
           string joinCode = await RelayService.Instance.GetJoinCodeAsync( allocation.AllocationId );
           Debug.Log( "Relay Server JoinCode: " + joinCode );
           
           m_TextServerCreated.text = "Server Created Successfully!";
           m_TextServerCreated.color = Color.green;
           
           m_TextJoinCode.text = "Room ID: " + joinCode;

           RelayServerData relayServerData = new RelayServerData( allocation, "dtls" );
           
           NetworkManager.Singleton.GetComponent < UnityTransport >().
                          SetRelayServerData(relayServerData );

           NetworkManager.Singleton.StartHost();
       }
       catch( RelayServiceException e )
       {
           m_TextServerCreated.text = "Error Creating Server";
           m_TextServerCreated.color = Color.red;
           Debug.Log( "RelayServiceException: " + e );
       }
   }
   
   [ContextMenu("Join Relay")]
   public async void JoinRelay( )
   {
       try
       {
           string joinCode = m_InputTextJoinCode.text;
           joinCode = joinCode.Substring(0, 6);
           
           Debug.Log( "Join Relay with " + joinCode );


           JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync( joinCode );

           RelayServerData relayServerData = new RelayServerData( joinAllocation, "dtls" );
           
           NetworkManager.Singleton.GetComponent < UnityTransport >().
                          SetRelayServerData(relayServerData );

           NetworkManager.Singleton.StartClient();
       }
       catch( RelayServiceException e )
       {
           Debug.Log( "RelayServiceException: " + e );
       }
   }

   [ContextMenu("Stop Relay")]
   public async void StopRelay()
   {
       try
       {
           AuthenticationService.Instance.SignedOut  += () =>
           {
               Debug.Log( "Signed out " + AuthenticationService.Instance.PlayerId );

           };
           NetworkManager.Singleton.Shutdown(  );
           Debug.Log( "Shutdown with ID: " + AuthenticationService.Instance.PlayerId );
           
           
       }
       catch( RelayServiceException e )
       {
           Debug.Log( "RelayServiceException: " + e );

           throw;
       }
   }

}
