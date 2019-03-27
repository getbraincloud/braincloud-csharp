using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestRTT : TestFixtureBase
    {
		//these tests also test requestClientConnection because its called first in EnableRtt. Authenticates and registers automatically.
        [Test]
        public void TestRTTEnableDisableRTTWithTCP()
        {
            TestResult tr = new TestResult(_bc);
            _bc.Client.EnableRTT(eRTTConnectionType.TCP, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            TestResult tr2 = new TestResult(_bc);
            _bc.Client.DisableRTT();
            tr2.Run();

            TestResult tr3 = new TestResult(_bc);
            _bc.Client.EnableRTT(eRTTConnectionType.TCP, tr.ApiSuccess, tr.ApiError);
            tr3.Run();

			TestResult tr2 = new TestResult(_bc);
            _bc.Client.DisableRTT();
            tr2.Run();
        }

        [Test]
        public void TestRTTEnableDisableRTTWithWS()
        {
            TestResult tr = new TestResult(_bc);
            _bc.Client.EnableRTT(eRTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            TestResult tr2 = new TestResult(_bc);
            _bc.Client.DisableRTT();
            tr2.Run();

            TestResult tr3 = new TestResult(_bc);
            _bc.Client.EnableRTT(eRTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr3.Run();

			TestResult tr2 = new TestResult(_bc);
            _bc.Client.DisableRTT();
            tr2.Run();
        } 

    }
}