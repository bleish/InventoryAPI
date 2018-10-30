using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace InventoryAPI.OperationFilters
{
    public class GeneralResponsesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // Produces

            operation.Produces.Clear();

            // Consumes

            operation.Consumes.Clear();
            
            // Responses

            operation.Responses.Add("500", new Response { Description = "There was an internal server error." });

            if (!context.MethodInfo.GetCustomAttributes(true).OfType<HttpGetAttribute>().Any())
            {
                operation.Responses.Remove("200");
            }

            var hasBodyParameters = false;
            var hasIdParameters = false;
            var hasQueryParameters = false;

            foreach (var param in context.ApiDescription.ParameterDescriptions)
            {
                if (param.Source == BindingSource.Body)
                {
                    hasBodyParameters = true;
                }
                else if (param.Source == BindingSource.Path)
                {
                    hasIdParameters = true;
                }
                else if (param.Source == BindingSource.Query || param.Source == BindingSource.ModelBinding)
                {
                    hasQueryParameters = true;
                }
            }

            // Other responses based on the possible parameters.

            if (hasBodyParameters)
            {
                operation.Responses.Add("400", new Response { Description = "The body is malformed or invalid." });
                operation.Consumes.Add("application/json");
            }
            else if (hasQueryParameters)
            {
                operation.Responses.Add("400", new Response { Description = "The query is malformed or invalid." });
            }
            
            if (hasIdParameters)
            {
                operation.Responses.Add("404", new Response { Description = "The ID in the path references a resource that doesn't exist." });
            }
        }
    }
}