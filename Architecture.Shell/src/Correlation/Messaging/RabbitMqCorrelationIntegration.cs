using RabbitMQ.Client;

namespace Architecture.Shell.Correlation.Messaging;

/// <summary>
/// Helper class for RabbitMQ correlation integration
/// </summary>
public class RabbitMqCorrelationIntegration
{
    private readonly ICorrelationPropagator _propagator;

    public RabbitMqCorrelationIntegration(ICorrelationPropagator propagator)
    {
        _propagator = propagator ?? throw new ArgumentNullException(nameof(propagator));
    }

    /// <summary>
    /// Injects correlation ID into RabbitMQ message properties
    /// </summary>
    /// <param name="properties">Message properties</param>
    public void InjectCorrelationId(IBasicProperties properties)
    {
        _propagator.Inject(properties);
    }

    /// <summary>
    /// Extracts correlation ID from RabbitMQ message properties
    /// </summary>
    /// <param name="properties">Message properties</param>
    /// <returns>Correlation ID or null if not found</returns>
    public string? ExtractCorrelationId(IBasicProperties properties)
    {
        return _propagator.Extract(properties);
    }

    /// <summary>
    /// Creates correlation context from RabbitMQ message properties
    /// </summary>
    /// <param name="properties">Message properties</param>
    /// <param name="factory">Correlation context factory</param>
    /// <returns>Correlation context or null if correlation ID not found</returns>
    public CorrelationContext? CreateCorrelationContext(IBasicProperties properties, ICorrelationContextFactory factory)
    {
        var correlationId = ExtractCorrelationId(properties);
        return string.IsNullOrEmpty(correlationId) ? null : factory.Create(correlationId, "rabbitmq-header");
    }
}