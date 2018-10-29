using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;

namespace InventoryAPI.EndToEndTests.Helpers
{
    public class Result
    {
        public HttpResponseMessage Response { get; set; }
        public string Content { get; set; }

        public string ResourceId => Response.Headers.Location?.PathAndQuery.Split('/').LastOrDefault();

        public T GetTypedContent<T>()
        {
            return JsonConvert.DeserializeObject<T>(Content);
        }
    }
}