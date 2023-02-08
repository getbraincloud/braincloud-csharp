using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Tests.PlayMode
{
    public class TestExecutor
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
}