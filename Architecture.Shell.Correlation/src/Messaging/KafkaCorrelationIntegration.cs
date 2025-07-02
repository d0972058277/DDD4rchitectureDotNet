using System;
using Confluent.Kafka;

namespace Architecture.Shell.Correlation.Messaging;

/// <summary>
/// Helper class for Kafka correlation integration
/// </summary>
public class KafkaCorrelationIntegration
{
    private readonly ICorrelationPropagator _propagator;

    public KafkaCorrelationIntegration(ICorrelationPropagator propagator)
    {
        _propagator = propagator ?? throw new ArgumentNullException(nameof(propagator));
    }

    /// <summary>
    /// Injects correlation ID into Kafka headers
    /// </summary>
    /// <param name="headers">Kafka headers</param>
    public void InjectCorrelationId(Headers headers)
    {
        _propagator.Inject(headers);
    }

    /// <summary>
    /// Injects correlation ID into Kafka message
    /// </summary>
    /// <param name="message">Kafka message</param>
    public void InjectCorrelationId<TKey, TValue>(Message<TKey, TValue> message)
    {
        _propagator.Inject(message);
    }

    /// <summary>
    /// Extracts correlation ID from Kafka headers
    /// </summary>
    /// <param name="headers">Kafka headers</param>
    /// <returns>Correlation ID or null if not found</returns>
    public string? ExtractCorrelationId(Headers headers)
    {
        return _propagator.Extract(headers);
    }

    /// <summary>
    /// Extracts correlation ID from Kafka message
    /// </summary>
    /// <param name="message">Kafka message</param>
    /// <returns>Correlation ID or null if not found</returns>
    public string? ExtractCorrelationId<TKey, TValue>(Message<TKey, TValue> message)
    {
        return _propagator.Extract(message);
    }

    /// <summary>
    /// Creates correlation context from Kafka headers
    /// </summary>
    /// <param name="headers">Kafka headers</param>
    /// <param name="factory">Correlation context factory</param>
    /// <returns>Correlation context or null if correlation ID not found</returns>
    public CorrelationContext? CreateCorrelationContext(Headers headers, ICorrelationContextFactory factory)
    {
        var correlationId = ExtractCorrelationId(headers);
        return string.IsNullOrEmpty(correlationId) ? null : factory.Create(correlationId, "kafka-header");
    }

    /// <summary>
    /// Creates correlation context from Kafka message
    /// </summary>
    /// <param name="message">Kafka message</param>
    /// <param name="factory">Correlation context factory</param>
    /// <returns>Correlation context or null if correlation ID not found</returns>
    public CorrelationContext? CreateCorrelationContext<TKey, TValue>(Message<TKey, TValue> message,
        ICorrelationContextFactory factory)
    {
        var correlationId = ExtractCorrelationId(message);
        return string.IsNullOrEmpty(correlationId) ? null : factory.Create(correlationId, "kafka-header");
    }
}