Instructions for getting Nintendo SDK working with BrainCloud.

Before proceeding, ensure you have the following:
    - NintendoSDK installed.
        - Ensure you have UnityForNintendoSwitch added as well.
    - Unity Engine with Nintendo Switch hardware platform add on installed. (Installers can be found within UnityForNintendoSwitch folder)
    - Add Unity Package from NintendoSDK to your project (Found in folder NintendoEnvironment/UnityForNintendoSwitch/Plugins/NintendoSDKPlugin/Libraries) 

Setting up:

1 - Download BrainCloud unity package latest plugin from here:
    - https://github.com/getbraincloud/braincloud-csharp/releases
2 - Import BrainCloud unity package.
3 - Log into BrainCloud via brainCloud Unity Plugin with the brainCloud tab found at the top and go to Settings.
    - Enable the Nintendo Platform (Core App Info -> Platforms)
4 - Download NetworkDemo from this link (You will need to log into Nintendo Developer Portal):
    Direct Download -> https://developer.nintendo.com/documents/portlet_file_entry/23933/NetworkingDemo_v1.2.0.zip/8bb99576-cb13-4fad-9956-629190696a71
    If URL above doesn't work, look for the latest version here -> https://developer.nintendo.com/group/development/g1kr9vj6/forums/english/-/gts_message_boards/thread/288206262#1530128
4 - Unzip NetworkDemo
5 - Navigate to the Assets folder in Network Demo
6 - Copy AccountManager & NetworkManager folders to your project's asset folder. 
7 - Use the Example Enumerator function provided as a reference that is provided below.
8 - Once the ExternalID and NSA ID Token is saved, you can call AuthenticateNintendo() from BrainCloudWrapper.cs using those variables.



Example Function for getting the External ID and NSA ID Token

    using System.Text;
    using nn.account;
    using sdsg.NintendoSwitch;
    
    private string _nsaIdToken;
    private string _externalId;

    IEnumerator ProcessUser()
    {
#if UNITY_SWITCH
        Account.Initialize();
        
        //Grab user that is currently logged in
        if (AccountManager.Primary == null)
        {
            UserAccount selectedUser = AccountManager.SelectAccount();

            if (selectedUser == null)
            {
                Debug.Log("Cannot use networking without a primary account selected.");
                yield break;
            }

            AccountManager.Primary = selectedUser;
        }
        
        // GUIDELINE 0153: Updating the user account state to reflect whether the user is playing your application
        // You must have the user's account open for active gameplay so that the play time and history are correctly recorded.
        if (!AccountManager.Primary.IsOpen)
        {
            AccountManager.Primary.Open();
        }
        
        // Prompt the user if anything goes wrong when attempting to enable networking
        NetworkManager.IsSilentNetworking = false;
        
        // Verify that the user has a network connection
        NetworkUseRequest networkUseRequest = new NetworkUseRequest();
        yield return networkUseRequest.Submit();
        
        if (!NetworkManager.IsAvailable)
        {
            Debug.Log("Network is unavailable");
            yield break;
        }
        
        // Verify that the user is able to obtain an NSA ID token
        NsaIdTokenRequest nsaIdTokenRequest = new NsaIdTokenRequest(true);
        yield return nsaIdTokenRequest.Submit();

        if (nsaIdTokenRequest.RequestResult != NsaResult.Success)
        {
            Debug.Log("There was a problem with your account");
        }
        
        //Token and External ID needed for BrainCloud Authenticate call. 
        _nsaIdToken = Encoding.UTF8.GetString(nsaIdTokenRequest.Token);
        _externalId = nsaIdTokenRequest.NsaId.ToString();
        
        Debug.Log($"External ID: {_externalId}");
        Debug.Log($"NSA ID TOKEN: {_nsaIdToken}");
#else
        Debug.Log("This scene is only meant to be tested on Nintendo Switch device");
        yield return null;
#endif
    }