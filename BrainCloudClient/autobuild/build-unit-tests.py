import sys
import os
import shutil
import zipfile
import argparse
import re

sys.path.append("../../../Common/build/python")
import vstudio
import util

def cleanArtifacts(in_artifactsPath):
	print()
	print("Cleaning artifacts")
	shutil.rmtree(in_artifactsPath, True)
	os.mkdir(in_artifactsPath)
	return

def main():
	# general vars
	artifacts = os.path.abspath("artifacts")

	# msbuild vars
	projectPath = os.path.abspath(".." + os.sep + "solution" + os.sep + "vstudio")
	projectFile = projectPath + os.sep + "BrainCloudClient.csproj"
	targets = "Rebuild"

	# clean up old builds
	cleanArtifacts(artifacts)

	# make new builds
	config = "Release"
	switches = "/p:OutputDir=bin\\Release"
	vstudio.buildProject(projectFile, targets, config, in_switches=switches)
	
	unitTestProjectFile = projectPath + os.sep + "UnitTest" + os.sep + "UnitTest.csproj"
	vstudio.buildProject(unitTestProjectFile, targets, config, in_switches=switches)
	
	# the previous step puts all dlls into bin\Release of UnitTest solution... run tests from there
	return

main()



