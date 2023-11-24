# brainCloud C# Client Library

Thanks for checking out [brainCloud](https://getbraincloud.com/)! This repository contains the client library for brainCloud projects that make use of C# including **Unity** and **Godot**.

Navigate to:
- [Latest Release](#latest-release)
- [Unity Installation Guide](#unity-installation-guide)
- [Godot Installation Guide (C# Only)](#godot-installation-guide-c-only)
- [Troubleshooting](#troubleshooting)
- [brainCloud Client Q&A](#braincloud-client-qa)

Here are a few handy links to get you started:

- You can learn all about brainCloud and find a few tutorials here:
    - https://docs.braincloudservers.com/learn/introduction/
- The brainCloud API Reference can be found here:
    - https://docs.braincloudservers.com/api/introduction
- Check out our Unity Bootcamp tutorial series, a free video course on how to use brainCloud in the Unity engine:
    - https://bootcamp.braincloudservers.com/ 
- We have a repository full of examples done in Unity that can be found here:
    - https://github.com/getbraincloud/examples-unity/#braincloud-unity-examples
- And a repository for Godot examples here:
    - https://github.com/getbraincloud/examples-godot/#braincloud-godot-examples
- If you haven't signed up or you want to log into the brainCloud portal, you can do so here:
    - https://portal.braincloudservers.com/
- The git for the custom package to install via Unity's package manager can be found here:
    - https://github.com/getbraincloud/braincloud-unity-package/#braincloud-unity-custom-package

## Latest Release

[brainCloud 5.1.0](https://github.com/getbraincloud/braincloud-csharp/releases/tag/5.1.0/)

| Package | Description |
| ------- | ------------|
| [**brainCloudClient_csharp_5.1.0.zip**](https://github.com/getbraincloud/braincloud-csharp/releases/download/5.1.0/brainCloudClient_csharp_5.1.0.zip) | C#/Godot Projects |
| [**brainCloudClient_unity_5.1.0.unitypackage**](https://github.com/getbraincloud/braincloud-csharp/releases/download/5.1.0/brainCloudClient_unity_5.1.0.unitypackage) | Unity Projects |

## Unity Installation Guide

There are two methods to install the brainCloud library for your Unity project:

### Package Manager

With your Unity project open:
1. Open `Window > Package Manager`
2. In the new window, click the big **+** icon in the top-left, then click **Add package from git URL...**
3. In the **URL** field, paste, and then click **Add**, this link:
    - https://github.com/getbraincloud/braincloud-unity-package.git

After it installs you should see **Packages - bitHeads Inc** with **brainCloud** underneath it in the Package Manager.

When a new update gets pushed, you can simply hit the **Update** button in the bottom-right of the window.

### .unitypackage Installation

With your Unity Project open, open the **brainCloudClient_unity_X.X.X.unitypackage** file and click the import prompt.

**Note** that the plugin has gone through major updates since BrainCloud 4.7 release, please refer to the appropriate plugin guide. 

### Updating

Whether you update with the .unitypackage or via the Package Manager, make sure to delete your old brainCloud settings first: `BrainCloud > Resources > BrainCloudEditorSettings_X_X_X` and `BrainCloud > Resources > BrainCloudSettings_X_X_X`.

If you plan on swapping to the brainCloud custom package, you must delete the old root BrainCloud library files folder `Assets > BrainCloud` in your Unity project, as well as the brainCloud plugin files `Plugins > Android > brainCloudUnity` and `Plugins > iOS > RegionLocaleNative`. If you do not have any additional Plugins, then it is safe to delete the root Plugins folder.

### brainCloud Unity Plugin 4.6 and Older

Once you install with the .unitypackage file, you will need to configure a few settings from the brainCloud menu. If you don't see a brainCloud menu, click any menu bar entry to get Unity to refresh the list of menus.

1. Open brainCloud and select Settings

![screenshot](/screenshots/1_EditorSelectSettings.png?raw=true)

2. Signup or Login to brainCloud

![screenshot](/screenshots/2_Login.png)

3. Select your team and your app

![screenshot](/screenshots/3_SelectTeamAndApp.png?raw=true)

4. With your app selected, debug information will now appear in the debug tab when the game is running

![screenshot](/screenshots/4_ViewDebugContent.png?raw=true)

#### Upgrading to 4.7 and Newer

1. If you used or called upon the **BrainCloudSettingsDLL** or the **BrainCloudEditorSettingsDLL** before, these have been replaced with **BrainCloudPlugin** and **BrainCloudPluginEditor** respectively. You may have some new errors where you hadn't before. You will need to delete your current BrainCloud plugin scripts and make adjustments for the new plugin.

2. BrainCloudPlugin and BrainCloudPluginEditor now only have readable values for security purposes. They do not have writeable values, so you may need to change some of your logic. The readable values are:
    - DispatcherURL 
    - AppId 
    - AppSecret 
    - AppIdSecrets
    - AppVersion

2. The app version is now handled through the Player Settings.

3. You no longer need to Enable Logging in code, you can toggle it on and off logging with the check box in the plugin window. This needs to be done in before running your app. 

4. To sign into other apps you will need to sign out and sign in to the new app for security purposes.

5. The plugin now has a Version number we will update when future changes are made.

---

## Godot Installation Guide (C# Only)

Initial support has been implemented to make this library compatible for those developing C# projects in Godot. Here's a guide on how to get brainCloud up and running in a Godot project:

1. Download the release package **brainCloudClient_csharp_X.X.X.zip** and extract the **BrainCloud** folder into the Godot project directory. The client library should be visible in **FileSystem** window within the Godot Editor.

![screenshot](/screenshots/GodotProjectFileSystem.png)

2. Create a new script to act as the brainCloud manager; in the `_Ready()` function of this script, create a new `BrainCloudWrapper` and initialize the app with the appropriate app ID and secret by calling `BrainCloudWrapper.Init(url, secretKey, appId, version)`.

3. In order to receive responses, be sure to call `BrainCloudWrapper.Update()` from the `_Process(double delta)` function of this script.

```csharp
BrainCloudWrapper _bc = null;

public override void _Ready()
{
    _bc = new BrainCloudWrapper();
    _bc.Init(url, secretKey, appId, version);
    _bc.Client.EnableLogging(true);

    GD.Print("brainCloud client version: " + _bc.Client.BrainCloudClientVersion);
}

public override void _Process(double delta)
{
    _bc.Update();
}
```

4. This script is now ready to handle brainCloud requests throughout the rest of the app. Functions for authentication and other brainCloud services can be defined here and called from other scripts/scenes in your Godot project.

5. To make the script accessible from any scene, use Godot's **Autoload** feature. Go to `Project > Project Settings` from the Godot editor, then switch to the **Autoload** tab. From here, find the script by typing in its path, or clicking the directory icon to browse project files. Give the Autoload node a name (we used BCManager), then add it to the list.

![screenshot](/screenshots/GodotAutoloadSingleton.png)

The newly created script should be ready to act like a singleton/global variable! Any node/script/scene in the project can access this via `GetNode<BCManager>("/root/BCManager")` to make calls/references to this script, for example: `GetNode<BCManager>("/root/BCManager").RequestAnonymousAuthentication()`

---

## Troubleshooting

Here are a few common errors that you may see on your first attempt to connect to brainCloud:

- **App ID not set** — Verify you've set up the app ID and app secret correctly when initializing the BrainCloudWrapper or Client
- **Platform not enabled** — Verify you've enabled your platform on the brainCloud portal
    - If you're running from the Unity editor, you'll need to enable either **Windows** or **Mac OS** to run in the editor

If you're still having issues then log into the portal and give us a shout through the help system (bottom right icon with the question mark and chat bubble)!

---

## brainCloud Client Q&A

brainCloud is a ready-made back-end platform for the development of feature-rich games, apps and things. brainCloud provides the features you need — along with comprehensive tools to support your team during development, testing and user support.

brainCloud consists of:
- Cloud Service – an advanced, Software-as-a-Service (SaaS) back-end
- Client Libraries – local client libraries (SDKs)
- Design Portal – a portal that allows you to design and debug your apps
- The brainCloud Architecture:

![architecture](/screenshots/bc-architecture.png?raw=true)

#### What's the difference between the BrainCloudWrapper and the BrainCloudClient?

**BrainCloudWrapper** contains quality of life improvements and handles the initialization and update for **BrainCloudClient**. It will act as a bridge between your app's front-end and the brainCloud APIs. It also helps with some platform specific functionality such as serializing the user's login ID on an Android or iOS device.

![wrapper](/screenshots/bc-wrapper.png?raw=true)

It is recommended to use the Wrapper by default!

#### How do I initialize brainCloud?

The following example shows how you can initialize brainCloud through the `BrainCloudWrapper`:
```csharp
BrainCloudWrapper _bc = null; // You may want this to be a static instance

public void InitializeBrainCloud()
{
    _bc = new BrainCloudWrapper("YourWrapperName"); // You can optionally set a wrapper name
    _bc.Init(url, secretKey, appId, version);
}
```

Then make sure in an Update loop you call `BrainCloudWrapper.Update()` so that brainCloud requests and responses are managed properly:

```csharp
public void YourUpdateLoop(double dt)
{
    _bc.Update();
}
```

This also works in Godot as seen in the [Godot Installation Guide](#godot-installation-guide-c-only) above.

In Unity `BrainCloudWrapper` is a **MonoBehaviour** object so you can either attach it to a GameObject in the editor or create it when needed:
```csharp
BrainCloudWrapper _bc = null;

private void Start()
{
    GameObject go = new GameObject();
    _bc = go.AddComponent<BrainCloudWrapper>();
    _bc.WrapperName = "YourWrapperName";
    _bc.Init(); // The app data is taken from the brainCloud Unity Plugin through the Settings config

    DontDestroyOnLoad(go); // Make sure the GameObject doesn't get destroyed!
}
```

Since it is a MonoBehaviour the Update function will be called automatically by Unity.

If you're initializing the Wrapper manually then your app's information can be found on the brainCloud portal in your app's dashboard under `App > Design > Core App Info > Application IDs`.

![wrapper](/screenshots/bc-ids.png?raw=true)

`BrainCloudWrapper.WrapperName` prefixes serialized operations that the Wrapper calls. You should set the wrapper name if you plan on having multiple instances of brainCloud running.

#### Newly upgraded?

If your app is already live, you should **NOT** specify the wrapper name otherwise the library will look in the wrong location for your user's stored anonymousID and profileID information. Only add a name if you intend to alter the save data.

#### App Version

Make sure you set the app version properly. Using a version less than your minimum app version on brainCloud will prevent the user from accessing the service until they update their app to the lastest version you have provided them.

![wrapper](/screenshots/bc-minVersions.png?raw=true)

#### How do I authenticate a user with brainCloud?

The simplest form of authenticating with brainCloud Wrapper is an Anonymous Authentication.

```csharp
_bc.AuthenticateAnonymous(successCallback, failureCallback);
```

This method will create an account, and continue to use a locally saved anonymous ID.

Your success and failure callbacks inherit from `IServerCallback` and contain the functions needed to react to the brainCloud Server response.

To login with a specfic anonymous ID, you can use the brainCloud Client:
```csharp
_bc.Client.AuthenticationService.setAnonymousId(anonymousId); // You can re-use an Anon ID
_bc.Client.AuthenticationService.setAnonymousId(_bc.Client.AuthenticationService.generateAnonymousId()); // Or generate a new one
_bc.Client.AuthenticationService.AuthenticateAnonymous(forceCreate, callback);
```

Setting `forceCreate` to false will ensure the user will only login to an existing account. Setting it to true, will allow the user to register a new account.

#### How do I attach an email to a user's brainCloud profile?

After having the user create an anonymous with brainCloud, they are probably going to want to attach an email or username, so their account can be accessed via another platform, or when their local data is discarded. You can do so like this:
```csharp
_bc.IdentityService.attachEmailIdentity(email, password, callback);
```

There are many authentication types. You can also merge profiles and detach idenities. See the brainCloud documentation for more information:
http://getbraincloud.com/apidocs/apiref/?java#capi-auth

#### TimeUtil

Most of our APIs suggest using UTC time, so we have added utility functions for better handling of local and UTC time. These extend **DateTime**'s and **DateTimeOffset**'s functionality when importing from the `BrainCloud` namespace.
```csharp
DateTime.UTCDateTimeToUTCMillis();          // Converts the DateTime UTC time value into a long representing milliseconds
long.UTCMillisToUTCDateTime();              // Converts long value (which should represent milliseconds) into a DateTime UTC time value
DateTime.LocalTimeToUTCTime();              // Converts a DateTime local time value into a DateTime UTC time value
DateTime.UTCTimeToLocalTime();              // Converts a DateTime UTC time value into a DateTime local time value
DateTimeOffset.DateTimeOffsetToUTCMillis(); // Converts the DateTimeOffset UTC time value into a long representing milliseconds
long.UTCMillisToDateTimeOffset();           // Converts long value (which should represent milliseconds) into a DateTimeOffset UTC time value
DateTimeOffset.LocalTimeToUTCTime();        // Converts a DateTimeOffset local time value into a DateTimeOffset UTC time value
DateTimeOffset.UTCTimeToLocalTime();        // Converts a DateTimeOffset UTC time value into a DateTimeOffset local time value
```

Example of use:
```csharp
DateTime date = DateTime.Now.LocalTimeToUTCTime();
long dateMilliseconds = date.UTCDateTimeToUTCMillis();
_bc.ScriptService.ScheduleRunScriptMillisUTC("scriptName", Helpers.CreateJsonPair("testParm1", 1), dateMilliseconds, success, failure); // Pass it into one of our calls that needs UTC time.
```

---

For more information on brainCloud and its services, please check out [brainCloud Learn](https://docs.braincloudservers.com/learn/introduction/) and [API Reference](https://docs.braincloudservers.com/api/introduction).
