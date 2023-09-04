using Architecture.Core;
using Project.WebApi;

namespace FluentValidation;

public static class RuleBuilderInitialExtensions
{
    public static IRuleBuilderInitial<T, TProperty> UseSpecification<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilderInitial, SpecificationBase<T> specification)
    {
        ruleBuilderInitial.Cascade(CascadeMode.Stop);

        foreach (var rule in specification.GetRules())
        {
            ruleBuilderInitial.Must((t, property) => rule.Validate(t)).WithMessage(rule.Message);
        }

        return ruleBuilderInitial;
    }

    public static AbstractValidator<T> UseSpecification<T>(this AbstractValidator<T> abstractValidator, SpecificationBase<T> specification)
    {
        var ruleBuilderInitial = abstractValidator.RuleFor(x => x)
            .Cascade(CascadeMode.Stop).Configure(c => c.PropertyName = MD5TypeName.Get<T>());

        foreach (var rule in specification.GetRules())
        {
            ruleBuilderInitial.Must(t => rule.Validate(t)).WithMessage(rule.Message);
        }

        return abstractValidator;
    }
}
