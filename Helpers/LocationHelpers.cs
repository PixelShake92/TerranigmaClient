using System.Collections.Generic;
using Archipelago.Core.Models;
using TerranigmaClient;

namespace Helpers
{
    public static class LocationHelpers
    {
        /// <summary>
        /// Build the location list for Archipelago monitoring
        /// </summary>
        public static List<ILocation> BuildLocationList(Dictionary<string, object> options = null)
        {
            return Locations.BuildLocationList(options);
        }
    }
}
