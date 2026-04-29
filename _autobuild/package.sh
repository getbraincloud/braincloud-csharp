#!/usr/bin/env bash
# =====================================================================
# Syncs source scripts and resource files from the base brainCloud
# C#/Unity Client Library to the Unity Package Manager version.
#
# - For files that already exist in the destination: copies only the
#   source file (the destination .meta is preserved).
# - For NEW files (not yet in the destination): copies BOTH the file
#   and its .meta so Unity assigns the same GUID.
# - For NEW folders: creates the folder and copies its .meta.
# - Reports orphaned files (exist in the package but have no matching
#   source) at the end. Nothing is deleted automatically.
#
# Usage:
#   ./package.sh <path-to-braincloud-unity-package>
#   ./package.sh <path-to-braincloud-unity-package> --dry-run
#   ./package.sh --help
#
# The script expects to live at:
#   <braincloud-csharp-repo>/autobuild/package.sh
# and sources files from ../BrainCloudClient/Assets relative to itself.
# The destination package path is supplied as a command-line argument
# and may be absolute or relative to the current working directory.
# =====================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SCRIPT_PATH="$SCRIPT_DIR/$(basename "${BASH_SOURCE[0]}")"
SCRIPT_NAME="$(basename "${BASH_SOURCE[0]}" .sh)"

# ─── Help ───
show_help() {
  cat <<EOF

Usage: $SCRIPT_NAME <path-to-braincloud-unity-package> [--dry-run]

Arguments:
  <path-to-braincloud-unity-package>
      Required. Absolute or relative path to the Package Manager
      version of braincloud-unity-package. Must contain Runtime/
      and Editor/ subfolders.

Options:
  --dry-run    Preview all actions without copying or creating anything.
  -h, --help   Show this help.

Examples:
  $SCRIPT_NAME ~/dev/braincloud-unity-package
  $SCRIPT_NAME ../../braincloud-unity-package --dry-run

This script must live at:
  <repo>/braincloud-csharp/autobuild/$SCRIPT_NAME.sh

EOF
  exit 0
}

# ─── Parse args (flags + one positional arg: package path) ───
DRY_RUN=0
PACKAGE_ARG=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --dry-run|-dry-run|--dryrun)
      DRY_RUN=1
      shift
      ;;
    -h|--help|-\?)
      show_help
      ;;
    -*)
      echo "[ERROR] Unknown option: $1"
      echo ""
      show_help
      ;;
    *)
      if [[ -z "$PACKAGE_ARG" ]]; then
        PACKAGE_ARG="$1"
        shift
      else
        echo "[ERROR] Unexpected argument: $1"
        echo ""
        show_help
      fi
      ;;
  esac
done

if [[ -z "$PACKAGE_ARG" ]]; then
  echo "[ERROR] Missing required argument: path to braincloud-unity-package."
  echo ""
  show_help
fi

# ─── Resolve roots ───
# Script lives at <braincloud-csharp-repo>/autobuild/.
# Source is ../BrainCloudClient/Assets relative to the script.
CSHARP_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
if [[ -z "$CSHARP_ROOT" ]]; then
  echo "[ERROR] Could not resolve braincloud-csharp repo root from script location."
  echo "        Script path: $SCRIPT_PATH"
  exit 1
fi
SRC_BASE="$CSHARP_ROOT/BrainCloudClient/Assets"
VERSION_CS="$SRC_BASE/BrainCloud/Client/BrainCloud/Version.cs"

# Normalise PACKAGE_ARG to an absolute path (if it exists), then strip any
# trailing slash.
if [[ -d "$PACKAGE_ARG" ]]; then
  PACKAGE_ARG="$(cd "$PACKAGE_ARG" && pwd)"
fi
DST_BASE="${PACKAGE_ARG%/}"

if [[ ! -d "$SRC_BASE" ]]; then
  echo "[ERROR] Source not found: $SRC_BASE"
  echo "        Script path:     $SCRIPT_PATH"
  echo "        Resolved parent: $CSHARP_ROOT"
  echo "        This script must live in <braincloud-csharp-repo>/autobuild/"
  echo "        so that ../BrainCloudClient/Assets resolves correctly."
  exit 1
fi
if [[ ! -d "$DST_BASE" ]]; then
  echo "[ERROR] Destination not found: $DST_BASE"
  echo "        Pass the correct path to the braincloud-unity-package folder."
  exit 1
fi
if [[ ! -d "$DST_BASE/Runtime" ]]; then
  echo "[ERROR] Destination does not look like a braincloud-unity-package:"
  echo "        missing \"Runtime\" subfolder in: $DST_BASE"
  exit 1
fi
if [[ ! -d "$DST_BASE/Editor" ]]; then
  echo "[ERROR] Destination does not look like a braincloud-unity-package:"
  echo "        missing \"Editor\" subfolder in: $DST_BASE"
  exit 1
fi

# ─── Counters ───
COUNT_NEW=0
COUNT_UPDATED=0
COUNT_NEW_DIRS=0
ORPHAN_COUNT=0

# ─── Temp tracking files ───
TMPDIR_BC="$(mktemp -d "${TMPDIR:-/tmp}/bc-sync.XXXXXX")"
TRACKED_FILE="$TMPDIR_BC/tracked.txt"
ORPHAN_FILE="$TMPDIR_BC/orphans.txt"
: > "$TRACKED_FILE"
: > "$ORPHAN_FILE"

# Clean up temp files on any exit (including errors / Ctrl-C).
trap 'rm -rf "$TMPDIR_BC"' EXIT

echo ""
echo "=== braincloud-unity-package sync ==="
if [[ "$DRY_RUN" == "1" ]]; then
  echo "Mode:   DRY-RUN (no files will be modified)"
else
  echo "Mode:   APPLY"
fi
echo "Source: $SRC_BASE"
echo "Dest:   $DST_BASE"
echo ""

# =====================================================================
#  Helper functions
# =====================================================================

# Echo $1 relative to $2. Empty string if $1 doesn't live under $2.
rel_path() {
  local full="$1"
  local base="$2"
  [[ "$base" != */ ]] && base="$base/"
  if [[ "$full" == "$base"* ]]; then
    echo "${full#$base}"
  else
    echo ""
  fi
}

# Copy a file with a labeled output line. Bumps the relevant counter.
#   $1 = src, $2 = dst, $3 = label (NEW / UPDATE / NEW-META)
copy_file() {
  local src="$1"
  local dst="$2"
  local label="$3"

  if [[ "$label" == "UPDATE" ]]; then
    COUNT_UPDATED=$((COUNT_UPDATED + 1))
  else
    COUNT_NEW=$((COUNT_NEW + 1))
  fi

  if [[ "$DRY_RUN" == "1" ]]; then
    printf '[DRY][%s]    %s\n' "$label" "$dst"
  else
    if ! cp -f "$src" "$dst"; then
      echo "[ERROR] Copy failed: $src -> $dst"
      return 1
    fi
    printf '[%s]         %s\n' "$label" "$dst"
  fi
}

# Ensure a destination directory exists; always track its .meta (so
# pre-existing directories aren't reported as orphans), and copy the
# source .meta only if we just created the dir.
#   $1 = source dir (used to locate the folder .meta), $2 = dest dir
ensure_dir() {
  local src_dir="$1"
  local dst_dir="$2"

  echo "$dst_dir.meta" >> "$TRACKED_FILE"

  if [[ -d "$dst_dir" ]]; then
    return 0
  fi

  COUNT_NEW_DIRS=$((COUNT_NEW_DIRS + 1))

  if [[ "$DRY_RUN" == "1" ]]; then
    printf '[DRY][MKDIR]     %s\n' "$dst_dir"
  else
    if ! mkdir -p "$dst_dir"; then
      echo "[ERROR] Failed to create directory: $dst_dir"
      return 1
    fi
    printf '[MKDIR]          %s\n' "$dst_dir"
  fi

  if [[ -f "$src_dir.meta" ]]; then
    copy_file "$src_dir.meta" "$dst_dir.meta" "NEW-META"
  else
    echo "[WARN] Source folder has no .meta: $src_dir"
  fi
}

# Walk up from $1 (treated as a descendant path) adding each ancestor
# directory's .meta to the tracked list. Stops at DST_BASE or the
# filesystem root.
track_ancestor_metas() {
  local current
  current="$(dirname "$1")"
  while [[ -n "$current" && "$current" != "/" && "$current" != "." && "$current" != "$DST_BASE" ]]; do
    echo "$current.meta" >> "$TRACKED_FILE"
    current="$(dirname "$current")"
  done
}

# Sync one file while preserving its position under src_base within
# dst_base. Copies the .meta only if the destination file is new.
#   $1 = full source file path, $2 = source base dir, $3 = dest base dir
sync_one_file() {
  local srcfile="$1"
  local src_base="$2"
  local dst_base="$3"

  local rel
  rel="$(rel_path "$srcfile" "$src_base")"
  if [[ -z "$rel" ]]; then
    echo "[ERROR] Could not compute relative path for $srcfile"
    return 1
  fi
  local dstfile="$dst_base/$rel"

  # Record both the file and its companion .meta as tracked.
  echo "$dstfile"        >> "$TRACKED_FILE"
  echo "$dstfile.meta"   >> "$TRACKED_FILE"

  if [[ -f "$dstfile" ]]; then
    copy_file "$srcfile" "$dstfile" "UPDATE"
  else
    # Safety net: ensure parent dir (normally already created by sync_dir).
    local parent
    parent="$(dirname "$dstfile")"
    if [[ ! -d "$parent" ]]; then
      if [[ "$DRY_RUN" == "1" ]]; then
        printf '[DRY][MKDIR]     %s\n' "$parent"
      else
        mkdir -p "$parent"
      fi
    fi
    copy_file "$srcfile" "$dstfile" "NEW"
    if [[ -f "$srcfile.meta" ]]; then
      copy_file "$srcfile.meta" "$dstfile.meta" "NEW-META"
    else
      echo "[WARN] Source file has no .meta: $srcfile"
    fi
  fi
}

# Sync a whole directory tree recursively.
#   $1 = source dir, $2 = destination dir
sync_dir() {
  local src="$1"
  local dst="$2"

  if [[ ! -d "$src" ]]; then
    echo "[WARN] Source missing, skipping: $src"
    return 0
  fi

  # Ensure dst root exists (and copy its .meta if new).
  ensure_dir "$src" "$dst"

  # Ensure all subdirectories exist - sorted so parents come before children.
  local subdir rel_sub
  while IFS= read -r -d '' subdir; do
    [[ "$subdir" == "$src" ]] && continue
    rel_sub="$(rel_path "$subdir" "$src")"
    [[ -z "$rel_sub" ]] && continue
    ensure_dir "$subdir" "$dst/$rel_sub"
  done < <(find "$src" -type d -print0 | sort -z)

  # Copy all non-.meta files.
  local f
  while IFS= read -r -d '' f; do
    [[ "$f" == *.meta ]] && continue
    sync_one_file "$f" "$src" "$dst"
  done < <(find "$src" -type f -print0)
}

# Sync an individual file where src and dst paths differ (e.g. renamed
# files or files placed in a different subtree on the package side).
#   $1 = full source file path, $2 = full dest file path
sync_file() {
  local src="$1"
  local dst="$2"

  if [[ ! -f "$src" ]]; then
    echo "[WARN] Source missing, skipping file: $src"
    return 0
  fi

  # Ensure parent dir of dest exists.
  local parent
  parent="$(dirname "$dst")"
  if [[ ! -d "$parent" ]]; then
    COUNT_NEW_DIRS=$((COUNT_NEW_DIRS + 1))
    if [[ "$DRY_RUN" == "1" ]]; then
      printf '[DRY][MKDIR]     %s\n' "$parent"
    else
      mkdir -p "$parent"
      printf '[MKDIR]          %s\n' "$parent"
    fi
  fi

  # Track ancestor folder metas (stops when we reach DST_BASE).
  track_ancestor_metas "$dst"

  # Track the file and its .meta.
  echo "$dst"      >> "$TRACKED_FILE"
  echo "$dst.meta" >> "$TRACKED_FILE"

  if [[ -f "$dst" ]]; then
    copy_file "$src" "$dst" "UPDATE"
  else
    copy_file "$src" "$dst" "NEW"
    if [[ -f "$src.meta" ]]; then
      copy_file "$src.meta" "$dst.meta" "NEW-META"
    fi
  fi
}

# Scan a destination subtree for files that are present on the package
# side but weren't tracked during the sync - those are orphans.
#   $1 = destination root to scan
#
# Currently every package file is synced from source; if the package
# ever grows files that live only on the package side and must not be
# flagged as orphans, add an allow-list check inside the loop.
scan_orphans() {
  local root="$1"
  [[ ! -d "$root" ]] && return 0

  local f
  while IFS= read -r -d '' f; do
    if ! grep -Fxq "$f" "$TRACKED_FILE"; then
      ORPHAN_COUNT=$((ORPHAN_COUNT + 1))
      echo "$f" >> "$ORPHAN_FILE"
    fi
  done < <(find "$root" -type f -print0)
}

# Read the version string from Version.cs (whatever GetVersion() returns)
# and write it into the "version" field of the package's package.json.
# Respects DRY_RUN. Emits a [VERSION] status line indicating whether the
# value was unchanged, updated, or would-update.
#
# Note: package.json's "version" field expects Semantic Versioning
# (MAJOR.MINOR.PATCH, e.g. 1.2.3). If the brainCloud version ever veers
# from that format we'll need to set the field manually.
update_package_version() {
  local pkg_file="$DST_BASE/package.json"

  if [[ ! -f "$VERSION_CS" ]]; then
    echo "[VERSION][WARN] Version.cs not found: $VERSION_CS"
    return 0
  fi
  if [[ ! -f "$pkg_file" ]]; then
    echo "[VERSION][WARN] package.json not found: $pkg_file"
    return 0
  fi

  # Grab the first `return "..."` line from Version.cs and keep only
  # whatever sits between the first pair of double quotes.
  local new_ver
  new_ver="$(grep 'return ' "$VERSION_CS" | head -n 1 \
             | sed -E 's/.*return[[:space:]]+"([^"]+)".*/\1/')"
  if [[ -z "$new_ver" ]]; then
    echo "[VERSION][WARN] Could not parse a version string from Version.cs"
    return 0
  fi

  # Find the current version in package.json.
  local old_ver
  old_ver="$(grep '"version"' "$pkg_file" | head -n 1 \
             | sed -E 's/.*"version"[[:space:]]*:[[:space:]]*"([^"]+)".*/\1/')"
  if [[ -z "$old_ver" ]]; then
    echo '[VERSION][WARN] Could not find "version" field in package.json'
    return 0
  fi

  if [[ "$old_ver" == "$new_ver" ]]; then
    echo "[VERSION]        unchanged ($new_ver) in package.json"
    return 0
  fi

  if [[ "$DRY_RUN" == "1" ]]; then
    echo "[DRY][VERSION]   package.json $old_ver -> $new_ver"
    return 0
  fi

  # Rewrite only the "version" line. Use '#' as the sed delimiter so a
  # version containing '/' (unlikely but possible) wouldn't break us.
  local tmp_out="$TMPDIR_BC/package.json.new"
  sed -E "s#(\"version\"[[:space:]]*:[[:space:]]*\")[^\"]+(\")#\1$new_ver\2#" \
      "$pkg_file" > "$tmp_out"

  # Safety: refuse to overwrite with an empty file.
  if [[ ! -s "$tmp_out" ]]; then
    echo "[ERROR] package.json rewrite produced empty output - aborting update."
    return 1
  fi

  if ! mv -f "$tmp_out" "$pkg_file"; then
    echo "[ERROR] Failed to update package.json"
    return 1
  fi
  echo "[VERSION]        package.json $old_ver -> $new_ver"
}

# =====================================================================
#  Main flow
# =====================================================================

# ─── Directory mappings (recursive) ───
# If we need a new folder to sync from the /BrainCloud folder then we can add it here.
sync_dir "$SRC_BASE/BrainCloud/Client"                   "$DST_BASE/Runtime/Client"
sync_dir "$SRC_BASE/BrainCloud/JsonFx"                   "$DST_BASE/Runtime/JsonFx"
sync_dir "$SRC_BASE/BrainCloud/ModernHttpClient"         "$DST_BASE/Runtime/ModernHttpClient"
sync_dir "$SRC_BASE/BrainCloud/Nintendo"                 "$DST_BASE/Runtime/Nintendo"
sync_dir "$SRC_BASE/BrainCloud/UnityWebSocketsForWebGL"  "$DST_BASE/Runtime/UnityWebsocketsForWebGL"
sync_dir "$SRC_BASE/BrainCloud/Unity/Editor/BCResources" "$DST_BASE/Editor/BCResources"
sync_dir "$SRC_BASE/Plugins"                             "$DST_BASE/Runtime/Plugins"

# ─── Individual file mappings (paths differ between src/dst) ───
# Note: brainCloud.asmdef in source becomes braincloud.asmdef (lower-case) in the package.
sync_file "$SRC_BASE/BrainCloud/brainCloud.asmdef"                       "$DST_BASE/Runtime/braincloud.asmdef"
sync_file "$SRC_BASE/BrainCloud/Unity/BrainCloudPlugin.dll"              "$DST_BASE/Runtime/Plugins/BrainCloudPlugin.dll"
sync_file "$SRC_BASE/BrainCloud/Unity/Editor/BrainCloudPluginEditor.dll" "$DST_BASE/Editor/Plugins/BrainCloudPluginEditor.dll"

# ─── Orphan scan ───
scan_orphans "$DST_BASE/Runtime"
scan_orphans "$DST_BASE/Editor"

# ─── package.json version sync (Version.cs -> package.json) ───
update_package_version

echo ""
echo "========================================="
echo "Summary"
echo "========================================="
echo "  New files copied:       $COUNT_NEW"
echo "  Existing files updated: $COUNT_UPDATED"
echo "  New directories:        $COUNT_NEW_DIRS"
echo "  Orphaned files:         $ORPHAN_COUNT"

if [[ "$ORPHAN_COUNT" -gt 0 ]]; then
  echo ""
  echo "---- Orphaned files (in package but not in source) ----"
  cat "$ORPHAN_FILE"
  echo "-------------------------------------------------------"
  echo "(Reported only. Review and remove manually if desired.)"
fi

if [[ "$DRY_RUN" == "1" ]]; then
  echo ""
  echo "Dry-run complete. Re-run without --dry-run to apply."
fi

exit 0
