using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    public static class Helpers
    {
        /// <summary>
        /// Creates a properly formatted key/value json pair
        /// </summary>
        /// <param name="key"> Key </param>
        /// <param name="value"> Value </param>
        /// <returns> Formatted Json pair </returns>
        public static string CreateJsonPair(string key, string value)
        {
            return "{ \"" + key + "\" : \"" + value + "\"}";
        }

        public static string CreateJsonPair(string key, long value)
        {
            return "{ \"" + key + "\" : " + value + "}";
        }

        public static Dictionary<string, object> GetDataFromJsonResponse(string response)
        {
            Dictionary<string, object> responseObj = JsonReader.Deserialize(response) as Dictionary<string, object>;
            return GetDataFromJsonResponse(responseObj);
        }

        public static Dictionary<string, object> GetDataFromJsonResponse(Dictionary<string, object> response)
        {
            return response["data"] as Dictionary<string, object>;
        }
    }
}
