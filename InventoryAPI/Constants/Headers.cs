using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace InventoryAPI.Constants
{
    internal static class Headers
    {
        internal static class Location
        {
            internal const string Name = "Location";
            internal const string Description = "URI of the resource.";
            internal const string Type = "string";
            
            internal static Dictionary<string, Header> HeaderDictionary => new Dictionary<string, Header>
            {{
                Name, new Header
                {
                    Description = Description,
                    Type = Type
                }
            }};
        }
    }
}