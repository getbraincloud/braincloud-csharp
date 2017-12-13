# brainCloud Unity/Csharp

Thanks for downloading the brainCloud Unity / C# client library! Here are a few notes to get you started. Further information about the brainCloud API and a few example Tutorials can be found here:

http://apidocs.braincloudservers.com/

If you haven't signed up or you want to log into the brainCloud portal, you can do that here:

https://portal.braincloudservers.com/

## Installation Guide

To install the brainCloud library simply copy these two folders into the Assets folder of your Unity project:

- BrainCloudClient/src/BrainCloud
- BrainCloudClient/src/Plugins

Once installed, you will need to configure a few settings from the brainCloud menu. If you don't see a brainCloud menu, click any menu bar entry to get Unity to refresh the list of menus.

- Open brainCloud | Select Settings. [screenshot](https://raw.githubusercontent.com/getbraincloud/Unity-Csharp/development/screenshots/1_EditorSelectSettings.png)
- Login or Signup to brainCloud. [screenshot](https://raw.githubusercontent.com/getbraincloud/Unity-Csharp/development/screenshots/2_Login.png) | Click Disable to switch to Old System
	- If using old system, configure your Game Id and Game Secret to the values in the brainCloud Portal **Application Ids** section
- Select your team and your app. [screenshot](https://raw.githubusercontent.com/getbraincloud/Unity-Csharp/development/screenshots/3_SelectTeamAndApp.png)
- With your app selected, debug information will now appear in the debug tab when the game is running. [screenshot](https://raw.githubusercontent.com/getbraincloud/Unity-Csharp/development/screenshots/4_ViewDebugContent.png)


## First run
To check that everything is working, try running the default scene which is located here:

- BrainCloud/Unity/Scenes/Default.unity

You should see a dialog box asking for user/password. Enter anything you want here and you should get authenticated.

## Troubleshooting

Here are a few common errors that you may see on your first attempt to connect to brainCloud.

- **App id not set**: Verify you've set up the app id and app secret correctly.
- **Platform not enabled**: Verify you've enabled your platform on the portal. If you're running from the Unity editor, you'll need to enable either **Windows** or **Mac OS**.

If you're still having issues, log into the portal and give us a shout through the help system (bottom right icon with the question mark and chat bubble).


## brainCloud Summary

brainCloud is a ready-made back-end platform for the development of feature-rich games, apps and things. brainCloud provides the features you need – along with comprehensive tools to support your team during development, testing and user support.

brainCloud consists of:
- Cloud Service – an advanced, Software-as-a-Service (SaaS) back-end
- Client Libraries – local client libraries (SDKs)
- Design Portal – a portal that allows you to design and debug your apps
- brainCloud Architecture

![architecture](/screenshots/bc-architecture.png?raw=true)

## What's the difference between the brainCloud Wrapper and the brainCloud Client?
The wrapper contains quality of life improvement around the brainCloud Client. It may contain device specific code, such as serializing the user's login id on an Android or iOS device.
It is recommended to use the wrapper by default.

![wrapper](/screenshots/bc-wrapper.png?raw=true)

## How do I initialize brainCloud?
If using the wrapper use the following code.
```csharp
_bc = go.AddComponent<BrainCloudWrapper>();
_bc.WrapperName = _wrapperName; // optionally set a wrapper-name
_bc.Init(); // extra like _appId and _secret, is taken from the brainCloud Unity Plugin. See Installation Guide above
```

Your _appId, _secret, is set on the brainCloud dashboard. Under Design | Core App Info > Application IDs

![wrapper](/screenshots/bc-ids.png?raw=true)

_wrapperName prefixes saved operations that the wrapper will make. Use a _wrapperName if you plan on having multiple instances of brainCloud running.

_appVersion is the current version of our app. Having an _appVersion less than your minimum app version on brainCloud will prevent the user from accessing the service until they update their app to the lastest version you have provided them.

![wrapper](/screenshots/bc-minVersions.png?raw=true)

## How do I authenticate a user with brainCloud?
The simplest form of authenticating with brainCloud Wrapper is an Anonymous Authentication.
```csharp
_bc.AuthenticateAnonymous(_successCallback, _failureCallback, NULL);
```
This method will create an account, and continue to use a locally saved anonymous id.

Your _callback will inherit from IServerCallback and contain the functions needed to react to the brainCloud Server response.


To login with a specfic anonymous id, use the brainCloud client.
```csharp
_bc.Client.AuthenticationService.setAnonymousId(_anonymousId); // re-use an Anon id
_bc.Client.AuthenticationService.setAnonymousId(_bc.Client.AuthenticationService.generateAnonymousId()); // or generate a new one
_bc.Client.AuthenticationService.AuthenticateAnonymous(_forceCreate, _callback);
```
Setting _forceCreate to false will ensure the user will only login to an existing account. Setting it to true, will allow the user to register a new account

## How do I attach an email to a user's brainCloud profile?
After having the user create an anonymous with brainCloud, they are probably going to want to attach an email or username, so their account can be accessed via another platform, or when their local data is discarded.
Attaching email authenticate would look like this.
```csharp
_bc.IdentityService.attachEmailIdentity(_email, _password, _callback);
```
There are many authentication types. You can also merge profiles and detach idenities. See the brainCloud documentation for more information:
http://getbraincloud.com/apidocs/apiref/?java#capi-auth
