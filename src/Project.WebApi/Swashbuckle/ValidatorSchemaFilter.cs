using FluentValidation;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Project.WebApi.Swashbuckle;

public class ValidatorSchemaFilter : ISchemaFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidatorSchemaFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(context.Type);

        using var scope = _serviceProvider.CreateScope();

        if (scope.ServiceProvider.GetService(validatorType) is not IEnumerable<IValidationRule> validationRules)
        {
            return;
        }

        foreach (var validationRule in validationRules)
        {
            IDictionary<string, IOpenApiExtension> extensions;

            if (TryGetPropertyName(schema, validationRule, out var propertyName))
            {
                extensions = schema.Properties[propertyName].Extensions;
            }
            else
            {
                extensions = schema.Extensions;
            }

            foreach (var component in validationRule.Components)
            {
                var message = component.GetUnformattedErrorMessage();

                extensions[$"x-rule-{extensions.Count + 1}"] = new Microsoft.OpenApi.Any.OpenApiString(message);
            }
        }
    }

    private static bool TryGetPropertyName(OpenApiSchema schema, IValidationRule validationRule, out string? propertyName)
    {
        foreach (var property in schema.Properties)
        {
            if (property.Key == validationRule.PropertyName)
            {
                propertyName = property.Key;
                return true;
            }
        }

        propertyName = null;
        return false;
    }
}
