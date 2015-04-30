import sys
import os
import shutil
import zipfile
import argparse
import re

sys.path.append("../../../../../../projGameLibs/Development/build/python")
import vstudio
import util

def cleanArtifacts(in_artifactsPath):
	print()
	print("Cleaning artifacts")
	shutil.rmtree(in_artifactsPath, True)
	os.mkdir(in_artifactsPath)
	return

def stampVersion(in_version):
	with open("BrainCloudClient.tmp", "wt") as fout:
		with open("../BrainCloudAdaptor/BrainCloudClient.cs", "rt") as fin:
			for line in fin:
				line = re.sub(r"s_brainCloudClientVersion = .*", "s_brainCloudClientVersion = \"" + in_version + "\";", line);
				fout.write(line)

	# this will fail if file is read-only
	shutil.move("BrainCloudClient.tmp", "../BrainCloudAdaptor/BrainCloudClient.cs")
	return

def stampReadme(in_platform, in_version):
	readmefile = "../../../Common/docs/README.TXT"; 
	with open("readme.tmp", "wt") as fout:
		with open(readmefile, "rt") as fin:
			for line in fin:
				line = re.sub(r"Platform\:.*", "Platform: " + in_platform, line);
				line = re.sub(r"Version\:.*", "Version: " + in_version, line);
				fout.write(line)

	# this will fail if file is read-only
	shutil.move("readme.tmp", readmefile)
	return

def main():
	# general vars
	artifacts = os.path.abspath("artifacts")

	parser = argparse.ArgumentParser(description="Run the build")
	parser.add_argument("--baseVersion", dest="baseVersion", action="store", required=True, help="Set the library version ie 1.5.0")
	args = parser.parse_args()

	scmRev = os.getenv("P4_CHANGELIST", "dev")
	version = args.baseVersion + "." + scmRev

	# msbuild vars
	projectPath = os.path.abspath(".." + os.sep + "BrainCloudAdaptor")
	prefabsPath = os.path.abspath(".." + os.sep + "BrainCloudPrefabs" + os.sep + "Assets")
	projectFile = projectPath + os.sep + "BrainCloud.csproj"
	targets = "Rebuild"


	# clean up old builds
	cleanArtifacts(artifacts)

	# stamp a version on the client library
	stampVersion(args.baseVersion)

	# stamp readme 
	stampReadme("Unity/C#", version)

	# make new builds
	config = "Release"
	switches = "/p:OutputDir=bin\\Release"
	vstudio.buildProject(projectFile, targets, config, in_switches=switches)

	config = "Release_Unity"
	switches = "/p:OutputDir=bin\\Release_Unity"
	vstudio.buildProject(projectFile, targets, config, in_switches=switches)
	
	# zip up builds
	with zipfile.ZipFile(artifacts + os.sep + "brainCloudClient_DotNet_" + version + ".zip", "w") as myzip:
		util.zipdir(projectPath + os.sep + "bin" + os.sep + "Release", myzip)
		myzip.write("../../../Common/docs/README.TXT", "README.TXT")

	with zipfile.ZipFile(artifacts + os.sep + "brainCloudClient_Unity_" + version + ".zip", "w") as myzip:
		myzip.write(projectPath + os.sep + "bin" + os.sep + "Release_Unity" + os.sep + "BrainCloud.dll", "Plugins" + os.sep + "BrainCloud" + os.sep + "BrainCloud.dll")
		myzip.write(projectPath + os.sep + "bin" + os.sep + "Release_Unity" + os.sep + "BrainCloud.pdb", "Plugins" + os.sep + "BrainCloud" + os.sep + "BrainCloud.pdb")
		myzip.write(projectPath + os.sep + "bin" + os.sep + "Release_Unity" + os.sep + "LitJson.dll", "Plugins" + os.sep + "BrainCloud" + os.sep + "LitJson.dll")

		util.zipdir(projectPath + os.sep + "bin" + os.sep + "Release_Unity" + os.sep + "iOS", myzip, "Plugins" + os.sep + "iOS", None, ["*.cs"])
		util.zipdir(projectPath + os.sep + "bin" + os.sep + "Release_Unity" + os.sep + "iOS", myzip, "Plugins" + os.sep + "BrainCloud" + os.sep + "Scripts", None, ["*.mm"])

		util.zipdir(prefabsPath + os.sep + "Editor", myzip, "Editor", None, ["*.meta", "Build.cs"])
		util.zipdir(prefabsPath + os.sep + "Plugins" + os.sep + "BrainCloud" + os.sep + "Prefabs", myzip, "Plugins" + os.sep + "BrainCloud" + os.sep + "Prefabs", None, ["*.meta"])
		util.zipdir(prefabsPath + os.sep + "Plugins" + os.sep + "BrainCloud" + os.sep + "Scripts", myzip, "Plugins" + os.sep + "BrainCloud" + os.sep + "Scripts", None, ["*.meta"])

		myzip.write("../../../Common/docs/README.TXT", "README.TXT")
	
	return

main()


