using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestCustomEntityService : TestFixtureBase
    {
        [Test]
        public void TestCreateCustomEntity()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.CreateCustomEntity(
                "sword001", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetCustomEntityPage()
        {
            //string context = "{\"test\": \"Testing\"}";
            //pass in context
            TestResult tr = new TestResult(_bc);

            _bc.ItemCatalogService.GetCustomEntityPage(
                context,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetCustomEntityPageOffset()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJjYXRlZ29yeSI6InN3b3JkIiwiZ2FtZUlkIjoiMjAwMDEifSwic29ydENyaXRlcmlhIjp7ImNyZWF0ZWRBdCI6MSwidXBkYXRlZEF0IjotMX0sInBhZ2luYXRpb24iOnsicm93c1BlclBhZ2UiOjUwLCJwYWdlTnVtYmVyIjoxfSwib3B0aW9ucyI6bnVsbH0";
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.GetCustomEntityPageOffset(
                context,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void ReadCustomEntity()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJjYXRlZ29yeSI6InN3b3JkIiwiZ2FtZUlkIjoiMjAwMDEifSwic29ydENyaXRlcmlhIjp7ImNyZWF0ZWRBdCI6MSwidXBkYXRlZEF0IjotMX0sInBhZ2luYXRpb24iOnsicm93c1BlclBhZ2UiOjUwLCJwYWdlTnVtYmVyIjoxfSwib3B0aW9ucyI6bnVsbH0";
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.ReadCustomEntity(
                context,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestUpdateCustomEntity()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJjYXRlZ29yeSI6InN3b3JkIiwiZ2FtZUlkIjoiMjAwMDEifSwic29ydENyaXRlcmlhIjp7ImNyZWF0ZWRBdCI6MSwidXBkYXRlZEF0IjotMX0sInBhZ2luYXRpb24iOnsicm93c1BlclBhZ2UiOjUwLCJwYWdlTnVtYmVyIjoxfSwib3B0aW9ucyI6bnVsbH0";
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.TestCustomEntity(
                context,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestUpdateCustomEntityFields()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJjYXRlZ29yeSI6InN3b3JkIiwiZ2FtZUlkIjoiMjAwMDEifSwic29ydENyaXRlcmlhIjp7ImNyZWF0ZWRBdCI6MSwidXBkYXRlZEF0IjotMX0sInBhZ2luYXRpb24iOnsicm93c1BlclBhZ2UiOjUwLCJwYWdlTnVtYmVyIjoxfSwib3B0aW9ucyI6bnVsbH0";
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.UpdateCustomEntityFields(
                context,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
