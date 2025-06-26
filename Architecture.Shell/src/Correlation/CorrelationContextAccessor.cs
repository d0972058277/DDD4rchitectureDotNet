namespace Architecture.Shell.Correlation;

/// <inheritdoc />
public class CorrelationContextAccessor : ICorrelationContextAccessor
{
    private static AsyncLocal<CorrelationContext> _correlationContext = new();

    /// <inheritdoc />
    public CorrelationContext? CorrelationContext
    {
        get => _correlationContext.Value;
        set => _correlationContext.Value = value;
    }
}