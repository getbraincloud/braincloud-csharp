@echo off
:: =====================================================================
:: Copies the appropriate folders from the brainCloud source to create
:: the C# Release .zip file. It will then Export our .unitypackage file
:: as well using the UNITY_EXE path set below.
::
:: - Wipes autobuild\staging\BrainCloud\ before copying so removed or
::   renamed files don't linger as stale content.
:: - Preserves README.txt (or anything else) sitting directly in
::   autobuild\staging\ - only the BrainCloud subfolder is touched.
::
:: USAGE:
::   release.bat
::
:: This script must live at:
::   <braincloud-csharp-repo>\autobuild\release.bat
:: =====================================================================

setlocal EnableDelayedExpansion

:: =====================================================================
::  CONFIG - update these when the Unity version changes
:: =====================================================================
set "UNITY_EXE=C:\Program Files\Unity\Hub\Editor\6000.0.68f1\Editor\Unity.exe"

set "SCRIPT_DIR=%~dp0"

:: ─── Resolve <repo>\braincloud-csharp root (parent of autobuild) ───
set "CSHARP_ROOT="
pushd "!SCRIPT_DIR!.." >nul 2>&1
if not errorlevel 1 (
  set "CSHARP_ROOT=!CD!"
  popd >nul 2>&1
)
if not defined CSHARP_ROOT (
  echo [ERROR] Could not resolve braincloud-csharp root.
  echo         Script dir: !SCRIPT_DIR!
  endlocal & exit /b 1
)

set "SRC_BASE=!CSHARP_ROOT!\BrainCloudClient\Assets\BrainCloud"
set "STAGING=!SCRIPT_DIR!staging"
set "DST_BASE=!STAGING!\BrainCloud"

if not exist "!SRC_BASE!" (
  echo [ERROR] Source not found: !SRC_BASE!
  endlocal & exit /b 1
)
if not exist "!STAGING!" (
  echo [ERROR] Staging folder not found: !STAGING!
  echo         Create it first ^(e.g. with a README.txt inside^).
  endlocal & exit /b 1
)

echo.
echo === braincloud-csharp -^> staging release ===
echo Source: !SRC_BASE!
echo Dest:   !DST_BASE!
echo.

:: ─── Wipe staging\BrainCloud\ so removed/renamed files don't linger ───
if exist "!DST_BASE!" (
  echo [CLEAN] !DST_BASE!
  rmdir /s /q "!DST_BASE!" >nul 2>&1
  if errorlevel 1 (
    echo [ERROR] Failed to clean destination: !DST_BASE!
    endlocal & exit /b 1
  )
)
mkdir "!DST_BASE!" >nul 2>&1

:: ─── Directories to copy ───
:: If we need a new folder to copy from the \BrainCloud folder then we can add it here.
call :CopyFolder Client           || goto :fail
call :CopyFolder JsonFx           || goto :fail
call :CopyFolder ModernHttpClient || goto :fail

:: ─── Strip all .meta files from the staging copy ───
echo [CLEAN] removing .meta files
del /s /q "!DST_BASE!\*.meta" >nul 2>&1

:: ─── Extract version from Version.cs for zip name ───
set "VERSION_CS=!SRC_BASE!\Client\BrainCloud\Version.cs"
set "VERSION="
set "_TMP_VER=%TEMP%\bc_ver_%RANDOM%.txt"
if exist "!VERSION_CS!" (
  findstr /C:"return " "!VERSION_CS!" > "!_TMP_VER!" 2>nul
  for /f "usebackq delims=" %%L in ("!_TMP_VER!") do (
    if not defined VERSION (
      set "_LINE=%%L"
      set "_TMP=!_LINE:*return =!"
      set "_TMP=!_TMP:;=!"
      set "VERSION=!_TMP:~1,-1!"
    )
  )
  del /q "!_TMP_VER!" >nul 2>&1
)

if defined VERSION (
  set "ZIP_PATH=!SCRIPT_DIR!brainCloudClient_csharp_!VERSION!.zip"
) else (
  echo [WARN] Could not extract version from Version.cs, using default zip name
  set "ZIP_PATH=!SCRIPT_DIR!brainCloudClient_csharp.zip"
)
set "_README=!STAGING!\README.txt"

echo [ZIP]   !ZIP_PATH!

if exist "!_README!" (
  powershell -NoProfile -NoLogo -Command "Compress-Archive -Path '!DST_BASE!', '!_README!' -DestinationPath '!ZIP_PATH!' -Force"
) else (
  echo [WARN] README.txt missing from staging, zipping BrainCloud only
  powershell -NoProfile -NoLogo -Command "Compress-Archive -Path '!DST_BASE!' -DestinationPath '!ZIP_PATH!' -Force"
)
if errorlevel 1 (
  echo [ERROR] Zip creation failed
  endlocal & exit /b 1
)

:: ─── Export .unitypackage via Unity CLI ───
if defined VERSION (
  set "UPKG_PATH=!SCRIPT_DIR!brainCloudClient_unity_!VERSION!.unitypackage"
) else (
  set "UPKG_PATH=!SCRIPT_DIR!brainCloudClient_unity.unitypackage"
)
set "UNITY_PROJECT=!CSHARP_ROOT!\BrainCloudClient"
set "UNITY_LOG=!SCRIPT_DIR!unity-export.log"

echo [UPKG]  !UPKG_PATH!
echo         Running Unity in batch mode, this may take a minute...

if not exist "!UNITY_EXE!" goto :UnityMissing

"!UNITY_EXE!" -batchmode -nographics -quit ^
  -projectPath "!UNITY_PROJECT!" ^
  -exportPackage "Assets/BrainCloud" "Assets/Plugins" "!UPKG_PATH!" ^
  -logFile "!UNITY_LOG!"

if errorlevel 1 (
  echo [WARN] Unity export failed, see log: !UNITY_LOG!
)
goto :UnityDone

:UnityMissing
echo [WARN] Unity.exe not found at:
echo        !UNITY_EXE!
echo        Edit UNITY_EXE at the top of the script. Skipping .unitypackage export.

:UnityDone

echo.
echo Done.
endlocal
exit /b 0


:fail
echo.
echo [ERROR] Staging release failed.
endlocal & exit /b 1


:CopyFolder
::  %~1 = folder name under BrainCloud\
  set "_NAME=%~1"
  set "_SRC=!SRC_BASE!\!_NAME!"
  set "_DST=!DST_BASE!\!_NAME!"

  if not exist "!_SRC!" (
    echo [WARN] Source folder missing, skipping: !_SRC!
    exit /b 0
  )

  echo [COPY]  !_NAME!
  xcopy "!_SRC!" "!_DST!\" /E /I /Y /Q >nul
  if errorlevel 1 (
    echo [ERROR] xcopy failed for !_NAME!
    exit /b 1
  )

exit /b 0
