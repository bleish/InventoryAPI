using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace InventoryAPI.OperationFilters
{
    public class GetResponsesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.MethodInfo.GetCustomAttributes(true).OfType<HttpGetAttribute>().Any())
            {
                return;
            }
            
            // See GeneralResponsesOperationFilter for other status codes that can get returned.
            operation.Produces.Clear();
            operation.Consumes.Add("application/json");
        }
    }
}