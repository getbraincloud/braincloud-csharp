#!/bin/bash

set -x
set -e

pushd ../BrainCloudAdaptor
for f in `find . -name \*.cs`; do
	astyle --suffix=none --style=allman --indent-namespaces "$f"
done
popd

