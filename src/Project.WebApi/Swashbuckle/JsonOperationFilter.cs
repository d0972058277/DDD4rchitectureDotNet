using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Project.WebApi.Swashbuckle;

public class JsonOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // 檢查是否有 ProducesAttribute。
        var hasProducesAttribute = context.MethodInfo
            .GetCustomAttributes(true)
            .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? Array.Empty<object>())
            .OfType<ProducesAttribute>()
            .Any();

        if (hasProducesAttribute) return;
        foreach (var response in operation.Responses)
        {
            response.Value.Content.Remove("text/plain");
            response.Value.Content.Remove("text/json");
        }
    }
}
