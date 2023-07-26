# brainCloud Unity Custom Package

This git repo will allow you to install the brainCloud library for your Unity project via the Package Manager. For more information, please see the [brainCloud Unity/Csharp](https://github.com/getbraincloud/braincloud-csharp/) git.

---

## Installation Guide

With your Unity project open:
1. Open `Window > Package Manager`
2. In the new window, click the big **+** icon in the top-left, then click **Add package from git URL...**
3. In the **URL** field, paste, and then click **Add**, this link:
    - https://github.com/getbraincloud/braincloud-unity-package.git

After it installs you should see **Packages - bitHeads Inc** with **brainCloud** underneath it in the Package Manager.

When a new update gets pushed, you can simply hit the **Update** button in the bottom-right of the window.

## Updating

Be sure to delete your old brainCloud settings before updating first: `BrainCloud > Resources > BrainCloudEditorSettings_X_X_X` and `BrainCloud > Resources > BrainCloudSettings_X_X_X`.

If you plan on swapping to the brainCloud custom package after installing the brainCloudClient_unity_X.X.X.unitypackage previously, you must delete the old root BrainCloud library files folder `Assets > BrainCloud` in your Unity project, as well as the brainCloud plugin files `Plugins > Android > brainCloudUnity` and `Plugins > iOS > RegionLocaleNative`. If you do not have any additional Plugins, then it is safe to delete the root Plugins folder.
