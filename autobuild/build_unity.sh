#!/bin/bash

set -e

if [ "$build_version" == "" ]; then
  echo "Must pass in build version"
  exit 1
fi

rm -rf artifacts
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
../autobuild/braincloudunity.unitypackage -quit


mkdir -p artifacts/brainCloudClient
cp docs/README.txt artifacts/brainCloudClient
sed -i "s/Platform.*/Platform\: C\#/" artifacts/brainCloudClient/README.TXT
sed -i "s/Version.*/Version\: $build_version/" artifacts/brainCloudClient/README.TXT

cp -r ../BrainCloudClient/Assets/BrainCloud artifacts/brainCloudClient
pushd artifacts/brainCloudClient

find -name "*.meta" -delete

find . -type f -name '*.cs' -exec sed -i '1i\
#define DOT_NET\n\n
' {} +

zip -r ../brainCloudClient_csharp_$build_version.zip .
popd
