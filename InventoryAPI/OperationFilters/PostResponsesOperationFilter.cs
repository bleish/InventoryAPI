using InventoryAPI.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace InventoryAPI.OperationFilters
{
    public class PostResponsesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.MethodInfo.GetCustomAttributes(true).OfType<HttpPostAttribute>().Any())
            {
                return;
            }
            
            // See GeneralResponsesOperationFilter for other status codes that can get returned.
            operation.Responses.Add("201", new Response
            {
                Description = "The resource was successfully created.",
                Headers = Headers.Location.HeaderDictionary
            });
        }
    }
}