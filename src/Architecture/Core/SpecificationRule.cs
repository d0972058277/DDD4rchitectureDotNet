namespace Architecture.Core;

public class SpecificationRule<T>
{
    public SpecificationRule(string message, Func<T, bool> validate)
    {
        Message = message;
        Validate = validate;
    }

    public string Message { get; }
    public Func<T, bool> Validate { get; }
}
