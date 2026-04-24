@echo off
:: =====================================================================
:: Syncs source scripts and resource files from the base brainCloud
:: C#/Unity Client Library to the Unity Package Manager version.
::
:: - For files that already exist in the destination: copies only the
::   source file (the destination .meta is preserved).
:: - For NEW files (not yet in the destination): copies BOTH the file
::   and its .meta so Unity assigns the same GUID.
:: - For NEW folders: creates the folder and copies its .meta.
:: - Reports orphaned files (exist in the package but have no matching
::   source) at the end. Nothing is deleted automatically.
::
:: USAGE:
::   package.bat <path-to-braincloud-unity-package>
::   package.bat <path-to-braincloud-unity-package> --dry-run
::   package.bat --help
::
:: The script expects to live at:
::   <braincloud-csharp-repo>\autobuild\package.bat
:: and sources files from ..\BrainCloudClient\Assets relative to itself.
:: The destination package path is supplied as a command-line argument
:: and may be absolute or relative to the current working directory.
:: =====================================================================

setlocal EnableDelayedExpansion

:: ─── Capture the script's own path BEFORE any shift ───
:: `shift` in the arg-parsing loop rewrites %0, so %~dp0 / %~f0 no longer
:: refer to this script once we start consuming args. Snapshot them here.
set "SCRIPT_PATH=%~f0"
set "SCRIPT_DIR=%~dp0"
set "SCRIPT_NAME=%~n0"

:: ─── Parse args (flags + one positional arg: package path) ───
set "DRY_RUN=0"
set "PACKAGE_ARG="

:parse_args
if "%~1"=="" goto :args_done
if /i "%~1"=="--dry-run"  ( set "DRY_RUN=1" & shift & goto :parse_args )
if /i "%~1"=="-dry-run"   ( set "DRY_RUN=1" & shift & goto :parse_args )
if /i "%~1"=="/dry-run"   ( set "DRY_RUN=1" & shift & goto :parse_args )
if /i "%~1"=="--dryrun"   ( set "DRY_RUN=1" & shift & goto :parse_args )
if /i "%~1"=="-h"     goto :show_help
if /i "%~1"=="--help" goto :show_help
if /i "%~1"=="/?"     goto :show_help
if not defined PACKAGE_ARG (
  :: Capture the package path, normalised to a fully qualified path.
  set "PACKAGE_ARG=%~f1"
  shift
  goto :parse_args
)
echo [ERROR] Unexpected argument: %~1
echo.
goto :show_help
:args_done

if not defined PACKAGE_ARG (
  echo [ERROR] Missing required argument: path to braincloud-unity-package.
  echo.
  goto :show_help
)

:: ─── Resolve roots ───
:: Script lives at <braincloud-csharp-repo>\autobuild\. Source is ..\BrainCloudClient\Assets.
:: Resolve the parent of the script directory via pushd so we get a
:: fully-qualified, canonicalised path (no ".." components).
set "CSHARP_ROOT="
pushd "!SCRIPT_DIR!.." >nul 2>&1
if not errorlevel 1 (
  set "CSHARP_ROOT=!CD!"
  popd >nul 2>&1
)
if not defined CSHARP_ROOT (
  echo [ERROR] Could not resolve braincloud-csharp repo root from script location.
  echo         Script path: !SCRIPT_PATH!
  endlocal & exit /b 1
)
set "SRC_BASE=!CSHARP_ROOT!\BrainCloudClient\Assets"
set "VERSION_CS=!SRC_BASE!\BrainCloud\Client\BrainCloud\Version.cs"

:: Strip trailing backslash from package arg if present, then set DST_BASE.
set "DST_BASE=!PACKAGE_ARG!"
if "!DST_BASE:~-1!"=="\" set "DST_BASE=!DST_BASE:~0,-1!"

if not exist "!SRC_BASE!" (
  echo [ERROR] Source not found: !SRC_BASE!
  echo         Script path:     !SCRIPT_PATH!
  echo         Resolved parent: !CSHARP_ROOT!
  echo         This script must live in <braincloud-csharp-repo>\autobuild\
  echo         so that ..\BrainCloudClient\Assets resolves correctly.
  endlocal & exit /b 1
)
if not exist "!DST_BASE!" (
  echo [ERROR] Destination not found: !DST_BASE!
  echo         Pass the correct path to the braincloud-unity-package folder.
  endlocal & exit /b 1
)
if not exist "!DST_BASE!\Runtime" (
  echo [ERROR] Destination does not look like a braincloud-unity-package:
  echo         missing "Runtime" subfolder in: !DST_BASE!
  endlocal & exit /b 1
)
if not exist "!DST_BASE!\Editor" (
  echo [ERROR] Destination does not look like a braincloud-unity-package:
  echo         missing "Editor" subfolder in: !DST_BASE!
  endlocal & exit /b 1
)

:: ─── Counters ───
set /a COUNT_NEW=0
set /a COUNT_UPDATED=0
set /a COUNT_NEW_DIRS=0
set /a ORPHAN_COUNT=0

:: ─── Temp tracking files ───
set "TMPDIR=%TEMP%\bc-sync-%RANDOM%%RANDOM%"
mkdir "!TMPDIR!" >nul 2>&1
set "TRACKED_FILE=!TMPDIR!\tracked.txt"
set "ORPHAN_FILE=!TMPDIR!\orphans.txt"
type nul >"!TRACKED_FILE!"
type nul >"!ORPHAN_FILE!"

echo.
echo === braincloud-unity-package sync ===
if "!DRY_RUN!"=="1" (
  echo Mode:   DRY-RUN ^(no files will be modified^)
) else (
  echo Mode:   APPLY
)
echo Source: !SRC_BASE!
echo Dest:   !DST_BASE!
echo.

:: ─── Directory mappings (recursive) ───
:: If we need a new folder to sync from the \BrainCloud folder then we can add it here.
call :SyncDir "!SRC_BASE!\BrainCloud\Client"                     "!DST_BASE!\Runtime\Client"
call :SyncDir "!SRC_BASE!\BrainCloud\JsonFx"                     "!DST_BASE!\Runtime\JsonFx"
call :SyncDir "!SRC_BASE!\BrainCloud\ModernHttpClient"           "!DST_BASE!\Runtime\ModernHttpClient"
call :SyncDir "!SRC_BASE!\BrainCloud\Nintendo"                   "!DST_BASE!\Runtime\Nintendo"
call :SyncDir "!SRC_BASE!\BrainCloud\UnityWebSocketsForWebGL"    "!DST_BASE!\Runtime\UnityWebsocketsForWebGL"
call :SyncDir "!SRC_BASE!\BrainCloud\Unity\Editor\BCResources"   "!DST_BASE!\Editor\BCResources"
call :SyncDir "!SRC_BASE!\Plugins"                               "!DST_BASE!\Runtime\Plugins"

:: ─── Individual file mappings (paths differ between src/dst) ───
:: Note: brainCloud.asmdef in source becomes braincloud.asmdef (lower-case) in the package.
call :SyncFile "!SRC_BASE!\BrainCloud\brainCloud.asmdef"                       "!DST_BASE!\Runtime\braincloud.asmdef"
call :SyncFile "!SRC_BASE!\BrainCloud\Unity\BrainCloudPlugin.dll"              "!DST_BASE!\Runtime\Plugins\BrainCloudPlugin.dll"
call :SyncFile "!SRC_BASE!\BrainCloud\Unity\Editor\BrainCloudPluginEditor.dll" "!DST_BASE!\Editor\Plugins\BrainCloudPluginEditor.dll"

:: ─── Orphan scan ───
call :ScanOrphans "!DST_BASE!\Runtime"
call :ScanOrphans "!DST_BASE!\Editor"

:: ─── package.json version sync (Version.cs -> package.json) ───
call :UpdatePackageVersion

echo.
echo =========================================
echo Summary
echo =========================================
echo   New files copied:       !COUNT_NEW!
echo   Existing files updated: !COUNT_UPDATED!
echo   New directories:        !COUNT_NEW_DIRS!
echo   Orphaned files:         !ORPHAN_COUNT!

if !ORPHAN_COUNT! gtr 0 (
  echo.
  echo ---- Orphaned files ^(in package but not in source^) ----
  type "!ORPHAN_FILE!"
  echo -------------------------------------------------------
  echo ^(Reported only. Review and remove manually if desired.^)
)

if "!DRY_RUN!"=="1" (
  echo.
  echo Dry-run complete. Re-run without --dry-run to apply.
)

:: ─── Cleanup ───
rmdir /s /q "!TMPDIR!" >nul 2>&1

endlocal
exit /b 0


:show_help
  echo.
  echo Usage: !SCRIPT_NAME! ^<path-to-braincloud-unity-package^> [--dry-run]
  echo.
  echo Arguments:
  echo   ^<path-to-braincloud-unity-package^>
  echo       Required. Absolute or relative path to the Package Manager
  echo       version of braincloud-unity-package. Must contain Runtime\
  echo       and Editor\ subfolders.
  echo.
  echo Options:
  echo   --dry-run    Preview all actions without copying or creating anything.
  echo   -h, --help   Show this help.
  echo.
  echo Examples:
  echo   !SCRIPT_NAME! C:\dev\braincloud-unity-package
  echo   !SCRIPT_NAME! ..\..\braincloud-unity-package --dry-run
  echo.
  echo This script must live at:
  echo   ^<repo^>\braincloud-csharp\autobuild\!SCRIPT_NAME!.bat
  echo.
  endlocal
  exit /b 0


:: =====================================================================
::  Subroutines
:: =====================================================================

:SyncDir
::  %~1 = source dir, %~2 = dest dir
  if not exist "%~1" (
    echo [WARN] Source missing, skipping: %~1
    exit /b 0
  )

  :: Ensure dst root exists (and copy its .meta if new)
  call :EnsureDir "%~1" "%~2"

  :: Ensure all subdirectories exist - sorted so parents come before children
  for /f "delims=" %%D in ('dir /ad /s /b "%~1" 2^>nul ^| sort') do (
    call :EnsureDirFromSrc "%%~fD" "%~1" "%~2"
  )

  :: Copy all non-.meta files
  for /r "%~1" %%F in (*) do (
    if /i not "%%~xF"==".meta" (
      call :SyncOneFile "%%~fF" "%~1" "%~2"
    )
  )
exit /b 0


:EnsureDir
::  %~1 = source dir (used to find the folder .meta)
::  %~2 = dest dir
::  Always track the folder .meta so an existing/created folder isn't
::  reported as an orphan. Only copy meta for brand-new folders.
  >>"!TRACKED_FILE!" echo %~2.meta
  if exist "%~2" exit /b 0
  set /a COUNT_NEW_DIRS+=1
  if "!DRY_RUN!"=="1" (
    echo [DRY][MKDIR]     %~2
  ) else (
    mkdir "%~2" >nul 2>&1
    if errorlevel 1 (
      echo [ERROR] Failed to create directory: %~2
      exit /b 1
    )
    echo [MKDIR]          %~2
  )
  if exist "%~1.meta" (
    call :CopyFile "%~1.meta" "%~2.meta" NEW-META
  ) else (
    echo [WARN] Source folder has no .meta: %~1
  )
exit /b 0


:EnsureDirFromSrc
::  %~1 = full source dir path
::  %~2 = source base dir
::  %~3 = dest base dir
  set "_FULL=%~1"
  call :RelPath _REL "!_FULL!" "%~2"
  if "!_REL!"=="" exit /b 0
  set "_DST=%~3\!_REL!"
  call :EnsureDir "!_FULL!" "!_DST!"
exit /b 0


:SyncOneFile
::  %~1 = full source file path
::  %~2 = source base dir
::  %~3 = dest base dir
  set "_SRCFILE=%~1"
  call :RelPath _REL "!_SRCFILE!" "%~2"
  if "!_REL!"=="" (
    echo [ERROR] Could not compute relative path for !_SRCFILE!
    exit /b 1
  )
  set "_DSTFILE=%~3\!_REL!"

  :: Record both the file and its companion .meta as tracked (for orphan scan)
  >>"!TRACKED_FILE!" echo !_DSTFILE!
  >>"!TRACKED_FILE!" echo !_DSTFILE!.meta

  if exist "!_DSTFILE!" (
    call :CopyFile "!_SRCFILE!" "!_DSTFILE!" UPDATE
  ) else (
    :: Safety: ensure parent dir (normally already created by SyncDir)
    for %%P in ("!_DSTFILE!") do set "_PARENT=%%~dpP"
    if "!_PARENT:~-1!"=="\" set "_PARENT=!_PARENT:~0,-1!"
    if not exist "!_PARENT!" (
      if "!DRY_RUN!"=="1" (
        echo [DRY][MKDIR]     !_PARENT!
      ) else (
        mkdir "!_PARENT!" >nul 2>&1
      )
    )
    call :CopyFile "!_SRCFILE!" "!_DSTFILE!" NEW
    if exist "!_SRCFILE!.meta" (
      call :CopyFile "!_SRCFILE!.meta" "!_DSTFILE!.meta" NEW-META
    ) else (
      echo [WARN] Source file has no .meta: !_SRCFILE!
    )
  )
exit /b 0


:SyncFile
::  %~1 = full source file path
::  %~2 = full dest file path
  set "_SRC=%~1"
  set "_DST=%~2"
  if not exist "!_SRC!" (
    echo [WARN] Source missing, skipping file: !_SRC!
    exit /b 0
  )

  :: Ensure parent dir of dest exists
  for %%P in ("!_DST!") do set "_PARENT=%%~dpP"
  if "!_PARENT:~-1!"=="\" set "_PARENT=!_PARENT:~0,-1!"
  if not exist "!_PARENT!" (
    set /a COUNT_NEW_DIRS+=1
    if "!DRY_RUN!"=="1" (
      echo [DRY][MKDIR]     !_PARENT!
    ) else (
      mkdir "!_PARENT!" >nul 2>&1
      echo [MKDIR]          !_PARENT!
    )
  )

  :: Track ancestor folder metas (stop when we reach DST_BASE)
  call :TrackAncestorMetas "!_DST!"

  :: Track both the file and its .meta
  >>"!TRACKED_FILE!" echo !_DST!
  >>"!TRACKED_FILE!" echo !_DST!.meta

  if exist "!_DST!" (
    call :CopyFile "!_SRC!" "!_DST!" UPDATE
  ) else (
    call :CopyFile "!_SRC!" "!_DST!" NEW
    if exist "!_SRC!.meta" (
      call :CopyFile "!_SRC!.meta" "!_DST!.meta" NEW-META
    )
  )
exit /b 0


:TrackAncestorMetas
::  %~1 = descendant path. Walks up parents and records each ancestor
::  directory's .meta file as tracked, stopping when it reaches DST_BASE.
  set "_TP=%~1"
:_tam_loop
  for %%P in ("!_TP!") do set "_TP=%%~dpP"
  if "!_TP:~-1!"=="\" set "_TP=!_TP:~0,-1!"
  if "!_TP!"=="" exit /b 0
  if /i "!_TP!"=="!DST_BASE!" exit /b 0
  :: stop at drive root (e.g. C:)
  if "!_TP:~1!"==":" exit /b 0
  >>"!TRACKED_FILE!" echo !_TP!.meta
  goto :_tam_loop


:CopyFile
::  %~1 = src, %~2 = dst, %~3 = label (NEW / UPDATE / NEW-META)
  if /i "%~3"=="UPDATE" (
    set /a COUNT_UPDATED+=1
  ) else (
    set /a COUNT_NEW+=1
  )
  if "!DRY_RUN!"=="1" (
    echo [DRY][%~3]    %~2
  ) else (
    copy /y "%~1" "%~2" >nul 2>&1
    if errorlevel 1 (
      echo [ERROR] Copy failed: %~1 -^> %~2
      exit /b 1
    )
    echo [%~3]         %~2
  )
exit /b 0


:RelPath
::  %~1 = output variable name
::  %~2 = full path
::  %~3 = base path
::  Sets !outVar! to full's path relative to base (no leading backslash).
::  Sets empty string if full equals base or base isn't a prefix of full.
  set "_F=%~2"
  set "_B=%~3"
  if not "!_B:~-1!"=="\" set "_B=!_B!\"
  call set "_R=%%_F:*!_B!=%%"
  if /i "!_R!"=="!_F!" set "_R="
  set "%~1=!_R!"
exit /b 0


:ScanOrphans
::  %~1 = dest root folder to scan
  if not exist "%~1" exit /b 0
  for /r "%~1" %%F in (*) do (
    set "_FILE=%%~fF"
    call :IsPackageOwned "!_FILE!" _ISKNOWN
    if "!_ISKNOWN!"=="0" (
      findstr /I /L /X /C:"!_FILE!" "!TRACKED_FILE!" >nul 2>&1
      if errorlevel 1 (
        set /a ORPHAN_COUNT+=1
        >>"!ORPHAN_FILE!" echo !_FILE!
      )
    )
  )
exit /b 0


:IsPackageOwned
::  %~1 = file path, %~2 = output var name
::  Sets !outVar! to 1 if the file is package-specific (not synced from
::  source) and should be excluded from the orphan report; else 0.
::  Currently there are no package-only files - brainCloud.asmdef IS
::  synced from source (see the explicit file mapping). Add entries
::  here if the package ever grows files that live only on the package
::  side and must not be flagged as orphans.
  set "_P=%~1"
  set "_OUT=0"
  set "%~2=!_OUT!"
exit /b 0


:UpdatePackageVersion
::  Reads the version string from Version.cs (the value returned by
::  GetVersion()) and writes it into the "version" field of the package's
::  package.json. Respects DRY_RUN. Prints a [VERSION] status line
::  indicating whether the value was unchanged, updated, or would-update.
::
::  Note: The "version" field in package.json expects the version to be
::  useing Semantic Versioning (SemVer) rules, formatted as
::  MAJOR.MINOR.PATCH (i.e. 1.2.3). If we veer from this formatting for
::  the brainCloud version then we will need to manually set the
::  "version" field after running this script.
::
::  Implementation detail: all findstr invocations write to temp files
::  first, then we read those files with plain `for /f`. Doing findstr
::  inside for/f backticks is fragile (cmd double-processes escapes for
::  chars like ^ and \", which can yield "FINDSTR: No search strings").
  set "_PKGFILE=!DST_BASE!\package.json"

  if not exist "!VERSION_CS!" (
    echo [VERSION][WARN] Version.cs not found: !VERSION_CS!
    exit /b 0
  )
  if not exist "!_PKGFILE!" (
    echo [VERSION][WARN] package.json not found: !_PKGFILE!
    exit /b 0
  )

  :: ─── Extract the version from Version.cs ───
  :: Grab the first `return "..."` line and take the value between the
  :: first pair of double quotes.
  set "_NEWVER="
  set "_VCS_LINES=!TMPDIR!\ver-cs.txt"
  findstr /C:"return " "!VERSION_CS!" > "!_VCS_LINES!" 2>nul
  for /f "usebackq delims=" %%L in ("!_VCS_LINES!") do (
    if not defined _NEWVER (
      set "_RLINE=%%L"
      for /f tokens^=2^ delims^=^" %%V in ("!_RLINE!") do (
        if not defined _NEWVER set "_NEWVER=%%V"
      )
    )
  )
  if not defined _NEWVER (
    echo [VERSION][WARN] Could not parse a version string from Version.cs
    exit /b 0
  )

  :: ─── Find the current version line in package.json ───
  :: Use a plain substring match on `version` - this JSON file has no
  :: other occurrence, and avoiding \"..\" sidesteps cmd quoting issues.
  set "_OLDVER="
  set "_VERLINE="
  set "_PKG_VERLINE=!TMPDIR!\ver-pkg.txt"
  findstr /N /C:"version" "!_PKGFILE!" > "!_PKG_VERLINE!" 2>nul
  for /f "usebackq tokens=1* delims=:" %%A in ("!_PKG_VERLINE!") do (
    if not defined _OLDVER (
      set "_VERLINE=%%A"
      set "_VERTXT=%%B"
      for /f tokens^=4^ delims^=^" %%V in ("!_VERTXT!") do (
        if not defined _OLDVER set "_OLDVER=%%V"
      )
    )
  )

  if not defined _OLDVER (
    echo [VERSION][WARN] Could not find "version" field in package.json
    exit /b 0
  )

  if "!_OLDVER!"=="!_NEWVER!" (
    echo [VERSION]        unchanged ^(!_NEWVER!^) in package.json
    exit /b 0
  )

  if "!DRY_RUN!"=="1" (
    echo [DRY][VERSION]   package.json !_OLDVER! -^> !_NEWVER!
    exit /b 0
  )

  :: ─── Rewrite package.json, replacing only the version line ───
  :: Dump every line (with line numbers) into a temp file first.
  :: We use `findstr /N /V /C:"<sentinel>"` (print lines NOT containing
  :: a string that can never appear) to number every line including
  :: empty ones, without relying on the ^ anchor - ^ is cmd's escape
  :: character and has proved unreliable here even inside quotes.
  set "_PKG_ALL=!TMPDIR!\pkg-all.txt"
  set "_PKG_NEVER=__SYNC_BC_PKG_IMPOSSIBLE_LINE_TOKEN_ZZZ__"
  findstr /N /V /C:"!_PKG_NEVER!" "!_PKGFILE!" > "!_PKG_ALL!" 2>nul

  :: Abort if we still produced nothing - without this guard an empty
  :: output would silently clobber package.json on the move below.
  for %%Z in ("!_PKG_ALL!") do set "_PKG_ALL_SIZE=%%~zZ"
  if "!_PKG_ALL_SIZE!"=="0" (
    echo [ERROR] Could not enumerate lines of package.json - aborting update.
    echo         Temp file: !_PKG_ALL!
    exit /b 1
  )

  set "_TMPOUT=!TMPDIR!\package.json.new"
  if exist "!_TMPOUT!" del /q "!_TMPOUT!" >nul 2>&1
  (
    for /f "usebackq tokens=1* delims=:" %%A in ("!_PKG_ALL!") do (
      if "%%A"=="!_VERLINE!" (
        echo(  "version": "!_NEWVER!",
      ) else (
        if "%%B"=="" (
          echo(
        ) else (
          echo(%%B
        )
      )
    )
  ) > "!_TMPOUT!"

  :: Sanity check: refuse to overwrite with an empty file.
  for %%Z in ("!_TMPOUT!") do set "_TMPSIZE=%%~zZ"
  if "!_TMPSIZE!"=="0" (
    echo [ERROR] package.json rewrite produced empty output - aborting update.
    exit /b 1
  )

  move /y "!_TMPOUT!" "!_PKGFILE!" >nul 2>&1
  if errorlevel 1 (
    echo [ERROR] Failed to update package.json
    exit /b 1
  )
  echo [VERSION]        package.json !_OLDVER! -^> !_NEWVER!
exit /b 0
