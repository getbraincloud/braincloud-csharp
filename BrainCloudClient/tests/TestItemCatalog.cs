using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestItemCatalog : TestFixtureBase
    {
        [Test]
        public void TestGetCatalogItemDefinition()
        {
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.GetCatalogItemDefinition(
                "sword001",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetCatalogItemsPage()
        {
            Dictionary<string, object> criteria = new Dictionary<string, object>();
            Dictionary<string, object> pagination = new Dictionary<string, object>();
            Dictionary<string, object> searchCriteria = new Dictionary<string, object>();
            Dictionary<string, object> sortCriteria = new Dictionary<string, object>();
            pagination.Add("rowsPerPage", 50);
            pagination.Add("pageNumber", 1);
            searchCriteria.Add("category", "sword");
            sortCriteria.Add("createdAt", 1);
            sortCriteria.Add("updatedAt", -1);
            criteria.Add("pagination", pagination);
            criteria.Add("searchCriteria", searchCriteria);
            criteria.Add("sortCriteria", sortCriteria);
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.GetCatalogItemsPage(
                criteria,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetCatalogItemsPageOffset()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJjYXRlZ29yeSI6InN3b3JkIiwiZ2FtZUlkIjoiMjAwMDEifSwic29ydENyaXRlcmlhIjp7ImNyZWF0ZWRBdCI6MSwidXBkYXRlZEF0IjotMX0sInBhZ2luYXRpb24iOnsicm93c1BlclBhZ2UiOjUwLCJwYWdlTnVtYmVyIjoxfSwib3B0aW9ucyI6bnVsbH0";
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.GetCatalogItemsPageOffset(
                context,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
