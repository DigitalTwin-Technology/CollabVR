using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Unity.Services.Vivox;

public class VivoxDTT : MonoBehaviour
{

    private int _PermissionAskedCount;

    [SerializeField]
    public string VoiceChannelName = "DTTChannel";
    
    
    void Start()
    {
        Debug.Log( "Starting VivoxDTT" );
        InitVivoxDTT();
    }

    public void InitVivoxDTT()
    {
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
        Debug.Log( "StartComplete" );

    }


    public void SignIntoVivox ()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
    bool IsAndroid12AndUp()
    {
        // android12VersionCode is hardcoded because it might not be available in all versions of Android SDK
        const int android12VersionCode = 31;
        AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int buildSdkVersion = buildVersionClass.GetStatic<int>("SDK_INT");

        return buildSdkVersion >= android12VersionCode;
    }

    string GetBluetoothConnectPermissionCode()
    {
        if (IsAndroid12AndUp())
        {
            // UnityEngine.Android.Permission does not contain the BLUETOOTH_CONNECT permission, fetch it from Android
            AndroidJavaClass manifestPermissionClass = new AndroidJavaClass("android.Manifest$permission");
            string permissionCode = manifestPermissionClass.GetStatic<string>("BLUETOOTH_CONNECT");

            return permissionCode;
        }
        return "";
    }
#endif

        bool IsMicPermissionGranted()
        {
            bool isGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (IsAndroid12AndUp())
        {
            // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission for all features to work
            isGranted &= Permission.HasUserAuthorizedPermission(GetBluetoothConnectPermissionCode());
        }
#endif
            return isGranted;
        }

        void AskForPermissions()
        {
            string permissionCode = Permission.Microphone;

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (_PermissionAskedCount == 1 && IsAndroid12AndUp())
        {
            permissionCode = GetBluetoothConnectPermissionCode();
        }
#endif
            _PermissionAskedCount++;
            Permission.RequestUserPermission(permissionCode);
        }

        bool IsPermissionsDenied()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission
        if (IsAndroid12AndUp())
        {
            return _PermissionAskedCount == 2;
        }
#endif
            return _PermissionAskedCount == 1;
        }

        //Actual code runs from here
        if (IsMicPermissionGranted())
        {
            //_VivoxManager.Login(transform.name.ToString());
            Debug.Log( "Trying to Login..." );
            LoginToVivox();
        }
        else
        {
            if (IsPermissionsDenied())
            {
                _PermissionAskedCount = 0;
                //_VivoxManager.Login(transform.name.ToString());
                Debug.Log( "PermissionsDenied - Trying to Login..." );

            }
            else
            {
                AskForPermissions();
                //_VivoxManager.Login(transform.name.ToString());      //NEED TO FIX !
                Debug.Log( "Asked For Permissions - Trying to Login..." );

            }
        }
    }
    
    async void LoginToVivox()
    {
        // TODO: Delete old Multiplayer System
        //await VivoxVoiceManager.Instance.InitializeAsync("local");
        var loginOptions = new LoginOptions()
        {
            DisplayName = "local", ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.FivePerSecond
        };
        await VivoxService.Instance.LoginAsync(loginOptions);
        Debug.Log( "trying to LoginAsync..." );

    }
    
    void OnUserLoggedIn()
    {
        
        Debug.Log( "trying to OnUserLoggedIn ..." );

        if( VivoxService.Instance.IsLoggedIn )
        {
            Debug.Log( "Successfully connected to Vivox" );
            Debug.Log( "Joining voice channel: " + VoiceChannelName );
            VivoxService.Instance.JoinGroupChannelAsync(VoiceChannelName, ChatCapability.AudioOnly);

        }
        else
        {
            Debug.Log( "Cannot sign into Vivox" );
        }
        
    }

    void OnUserLoggedOut()
    {
        Debug.Log( "Disconnected from voice channel: " + VoiceChannelName );
    }

 
    
}
