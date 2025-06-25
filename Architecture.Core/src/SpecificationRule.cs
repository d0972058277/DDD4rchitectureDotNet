namespace Architecture.Core;

public abstract class SpecificationRule<T>(string message, Func<T, bool> validate)
{
    public string Message { get; } = message;
    public Func<T, bool> Validate { get; } = validate;
}
