Instructions for getting Nintendo SDK working with BrainCloud.

Before proceeding, ensure you have the following:
    - NintendoSDK installed.
        - Ensure you have UnityForNintendoSwitch added as well.
    - Unity Engine with Nintendo Switch hardware platform add on installed. (Installers can be found within UnityForNintendoSwitch folder)

Setting up:

1 - Download BrainCloud unity package latest plugin from here:
    - https://github.com/getbraincloud/braincloud-csharp/releases
2 - Import BrainCloud unity package.

3 - Download NetworkDemo from this link:
    - https://developer.nintendo.com/documents/portlet_file_entry/23933/NetworkingDemo_v1.1.3.zip/08764e61-f3da-4c2b-813f-c1a365ce8707
4 - Unzip NetworkDemo
5 - Navigate to the Assets folder
6 - Copy AccountManager & NetworkManager folders to your project's asset folder. 
7a - Use the NetworkRequiredScript as a reference
7b - Use the Example Enumerator function provided as a reference that is provided below.
8 - Once the ExternalID and NSA ID Token is saved, you can call AuthenticateNintendo() from BrainCloudWrapper.cs using those variables.



Example Function for getting the External ID and NSA ID Token

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
        networkUseRequest = new NetworkUseRequest();
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