@echo off
rem brainCloud setup panel. Pass your project folder (where the .csproj is).
dotnet "%~dp0BrainCloudNative.Tool.dll" %*
