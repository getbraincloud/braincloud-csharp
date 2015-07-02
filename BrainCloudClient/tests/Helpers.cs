using System.Collections.Generic;

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
    }
}
