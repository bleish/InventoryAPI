using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace InventoryAPI.OperationFilters
{
    public class PutResponsesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.MethodInfo.GetCustomAttributes(true).OfType<HttpPutAttribute>().Any())
            {
                return;
            }
            
            // See GeneralResponsesOperationFilter for other status codes that can get returned.
            operation.Responses.Add("204", new Response { Description = "The resource was successfully updated." });
        }
    }
}