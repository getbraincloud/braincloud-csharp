using System.Collections.Generic;
using System.Threading;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud.JsonFx.Json;
using BrainCloud;

namespace BrainCloudTests
{
    public class BrainCloudTestsMain
    {
        static int Main(string[] args)
        {
            BrainCloudWrapper bc = new BrainCloudWrapper();

            TestResult tr = new TestResult(new BrainCloudWrapper());
            bc.Client.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
            if (tr.Run ())
            {
                // something
            }
            return 0;
        }
    }
}