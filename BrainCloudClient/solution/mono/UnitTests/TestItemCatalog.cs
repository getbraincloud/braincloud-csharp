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
        public void TestSwitchToChildProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToChildProfile(
                null,
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.PlayerStateService.DeleteUser(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
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
            pagination.add("rowsPerPage", 50);
            pagination.add("pageNumber", 1);
            searchCriteria.add("category", "sword");
            sortCriteria.add("createdAt", 1);
            sortCriteria.add("updatedAt", -1);
            criteria.add("pagination", pagination);
            criteria.add("searchCriteria", searchCriteria);
            criteria.add("sortCriteria", sortCriteria);
            TestResult tr = new TestResult(_bc);
            _bc.ItemCatalogService.GetCatalogItemsPage(
                context,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
