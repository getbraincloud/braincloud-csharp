#!/usr/bin/env bash
# =====================================================================
# Copies the appropriate folders from the brainCloud source to create
# the C# Release .zip file. It will then export our .unitypackage file
# as well using the UNITY_EXE path set below.
#
# - Wipes autobuild/staging/BrainCloud/ before copying so removed or
#   renamed files don't linger as stale content.
# - Preserves README.txt (or anything else) sitting directly in
#   autobuild/staging/ - only the BrainCloud subfolder is touched.
#
# USAGE:
#   ./release.sh
#
# This script must live at:
#   <braincloud-csharp-repo>/autobuild/release.sh
# =====================================================================

# =====================================================================
#  CONFIG - update this when the Unity version changes
# =====================================================================
UNITY_EXE="/Applications/Unity/Hub/Editor/6000.0.68f1/Unity.app/Contents/MacOS/Unity"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# ─── Resolve <repo>/braincloud-csharp root (parent of autobuild) ───
CSHARP_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
if [[ -z "$CSHARP_ROOT" ]]; then
  echo "[ERROR] Could not resolve braincloud-csharp root."
  echo "        Script dir: $SCRIPT_DIR"
  exit 1
fi

SRC_BASE="$CSHARP_ROOT/BrainCloudClient/Assets/BrainCloud"
STAGING="$SCRIPT_DIR/staging"
DST_BASE="$STAGING/BrainCloud"

if [[ ! -d "$SRC_BASE" ]]; then
  echo "[ERROR] Source not found: $SRC_BASE"
  exit 1
fi
if [[ ! -d "$STAGING" ]]; then
  echo "[ERROR] Staging folder not found: $STAGING"
  echo "        Create it first (e.g. with a README.txt inside)."
  exit 1
fi

echo ""
echo "=== braincloud-csharp -> staging release ==="
echo "Source: $SRC_BASE"
echo "Dest:   $DST_BASE"
echo ""

# ─── Helper: copy a named folder from SRC_BASE to DST_BASE ───
copy_folder() {
  local name="$1"
  local src="$SRC_BASE/$name"
  local dst="$DST_BASE/$name"

  if [[ ! -d "$src" ]]; then
    echo "[WARN] Source folder missing, skipping: $src"
    return 0
  fi

  echo "[COPY]  $name"
  if ! cp -R "$src" "$dst"; then
    echo "[ERROR] cp failed for $name"
    return 1
  fi
}

fail() {
  echo ""
  echo "[ERROR] Staging release failed."
  exit 1
}

# ─── Wipe staging/BrainCloud/ so removed/renamed files don't linger ───
if [[ -d "$DST_BASE" ]]; then
  echo "[CLEAN] $DST_BASE"
  if ! rm -rf "$DST_BASE"; then
    echo "[ERROR] Failed to clean destination: $DST_BASE"
    exit 1
  fi
fi
mkdir -p "$DST_BASE"

# ─── Directories to copy ───
# If we need a new folder to copy from the /BrainCloud folder then we can add it here.
copy_folder "Client"           || fail
copy_folder "JsonFx"           || fail
copy_folder "ModernHttpClient" || fail

# ─── Strip all .meta files from the staging copy ───
echo "[CLEAN] removing .meta files"
find "$DST_BASE" -type f -name "*.meta" -delete 2>/dev/null

# ─── Extract version from Version.cs for zip name ───
VERSION_CS="$SRC_BASE/Client/BrainCloud/Version.cs"
VERSION=""
if [[ -f "$VERSION_CS" ]]; then
  VERSION="$(grep 'return ' "$VERSION_CS" | head -n 1 \
             | sed -E 's/.*return[[:space:]]+"([^"]+)".*/\1/')"
fi

if [[ -n "$VERSION" ]]; then
  ZIP_PATH="$SCRIPT_DIR/brainCloudClient_csharp_$VERSION.zip"
else
  echo "[WARN] Could not extract version from Version.cs, using default zip name"
  ZIP_PATH="$SCRIPT_DIR/brainCloudClient_csharp.zip"
fi
README="$STAGING/README.txt"

echo "[ZIP]   $ZIP_PATH"

# Remove any existing zip so we don't accidentally append to it.
rm -f "$ZIP_PATH"

# zip works from cwd - cd into staging so BrainCloud/ and README.txt land
# at the archive root rather than carrying their absolute paths.
if [[ -f "$README" ]]; then
  ( cd "$STAGING" && zip -r -q "$ZIP_PATH" "BrainCloud" "README.txt" )
else
  echo "[WARN] README.txt missing from staging, zipping BrainCloud only"
  ( cd "$STAGING" && zip -r -q "$ZIP_PATH" "BrainCloud" )
fi
ZIP_RC=$?

if [[ $ZIP_RC -ne 0 ]]; then
  echo "[ERROR] Zip creation failed"
  exit 1
fi

# ─── Export .unitypackage via Unity CLI ───
if [[ -n "$VERSION" ]]; then
  UPKG_PATH="$SCRIPT_DIR/brainCloudClient_unity_$VERSION.unitypackage"
else
  UPKG_PATH="$SCRIPT_DIR/brainCloudClient_unity.unitypackage"
fi
UNITY_PROJECT="$CSHARP_ROOT/BrainCloudClient"
UNITY_LOG="$SCRIPT_DIR/unity-export.log"

echo "[UPKG]  $UPKG_PATH"
echo "        Running Unity in batch mode, this may take a minute..."

if [[ ! -x "$UNITY_EXE" ]]; then
  echo "[WARN] Unity executable not found at:"
  echo "       $UNITY_EXE"
  echo "       Edit UNITY_EXE at the top of the script. Skipping .unitypackage export."
else
# Reason why we have a specific list like this is to avoid including the Resources folders
# We will need to keep this updated if we add folders or files to the BrainCloud folder
  "$UNITY_EXE" -batchmode -nographics -quit \
    -projectPath "$UNITY_PROJECT" \
    -exportPackage "Assets/BrainCloud/brainCloud.asmdef" \
                   "Assets/BrainCloud/Client" \
                   "Assets/BrainCloud/JsonFx" \
                   "Assets/BrainCloud/ModernHttpClient" \
                   "Assets/BrainCloud/Nintendo" \
                   "Assets/BrainCloud/Unity/BrainCloudPlugin.dll" \
                   "Assets/BrainCloud/BCResources" \
                   "Assets/BrainCloud/Unity/Editor/BrainCloudPluginEditor.dll" \
                   "Assets/BrainCloud/UnityWebSocketsForWebGL" \
                   "Assets/Plugins" \
                   "$UPKG_PATH" \
    -logFile "$UNITY_LOG" \
    || echo "[WARN] Unity export failed, see log: $UNITY_LOG"
fi

echo ""
echo "Done."
exit 0
