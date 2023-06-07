using System.Collections;
using System.Collections.Generic;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public static class JenkinCommands
{
    public static void RunAllTests()
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.PlayMode
        };
         
        testRunnerApi.Execute(new ExecutionSettings(filter));
    }
}
