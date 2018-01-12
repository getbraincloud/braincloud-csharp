set -e

build_version=$1

if [ "$build_version" == "" ]; then
  echo "Must pass in build version"
  exit 1
fi

rm -rf artifacts
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
