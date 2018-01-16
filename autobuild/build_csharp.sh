
set -e
set -x

if [ "$build_version" == "" ]; then
  echo "Must pass in build version"
  exit 1
fi


rm -rf artifacts

# Create C# Client
mkdir -p artifacts/brainCloudClient
cp docs/README.txt artifacts/brainCloudClient
sed -i -e "s/Platform.*/Platform\: C\#/g" artifacts/brainCloudClient/README.TXT
sed -i -e "s/Version.*/Version\: $build_version/g" artifacts/brainCloudClient/README.TXT

cp -r ../BrainCloudClient/Assets/BrainCloud artifacts/brainCloudClient
pushd artifacts/brainCloudClient

find . -name "*.meta" -delete


find . -name '*.cs' -type f -exec sed -i -e '1i \
\#define DOT_NET \
\
' {} \;


zip -r ../brainCloudClient_csharp_$build_version.zip .
popd


# Create Unity Client
mkdir -p artifacts/brainCloudUnityPackage

cur_path=$(pwd)

/Applications/Unity/Unity.app/Contents/MacOS/Unity \
-batchmode \
-nographics \
-logFile /Users/jonm/Desktop/log-unity.txt \
-projectPath $cur_path/../BrainCloudClient/ \
-exportPackage \
Assets/BrainCloud/Client/BrainCloud \
Assets/BrainCloud/Client/BrainCloud/Common \
Assets/BrainCloud/Client/BrainCloud/Entity \
Assets/BrainCloud/Client/BrainCloud/Entity/Internal \
Assets/BrainCloud/Client/BrainCloud/Internal \
Assets/BrainCloud/JsonFx \
Assets/BrainCloud/LitJson \
Assets/BrainCloud/Resources \
Assets/BrainCloud/Unity/BrainCloudUnityPlugin/Editor \
Assets/BrainCloud/Unity/BrainCloudUnityPlugin/Scripts \
Assets/BrainCloud/Unity/Editor \
Assets/BrainCloud/Unity/Editor/Resources \
Assets/BrainCloud/Unity/Editor/Resources/Images \
Assets/BrainCloud/Unity/Prefabs \
Assets/BrainCloud/Unity/Scenes \
Assets/BrainCloud/Unity/Scripts \
Assets/BrainCloud/Unity/Scripts/HUD \
$cur_path/artifacts/brainCloudUnityPackage/brainCloudClient_unity_$build_version.unitypackage -quit

