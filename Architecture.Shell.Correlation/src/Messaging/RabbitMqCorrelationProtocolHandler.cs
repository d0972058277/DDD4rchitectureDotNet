using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Architecture.Shell.Correlation.Messaging;

/// <summary>
/// Protocol handler for RabbitMQ correlation ID propagation using IBasicProperties
/// </summary>
public class RabbitMqCorrelationProtocolHandler : ICorrelationProtocolHandler
{
    public Type MetadataType => typeof(IBasicProperties);

    public void Inject(object metadata, string headerName, string correlationId)
    {
        if (metadata is not IBasicProperties properties)
            return;

        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers[headerName] = correlationId;
    }

    public string? Extract(object metadata, string headerName)
    {
        if (metadata is not IBasicProperties properties || properties.Headers == null)
            return null;

        if (properties.Headers.TryGetValue(headerName, out var value))
            return value switch
            {
                string stringValue => stringValue,
                byte[] byteValue => System.Text.Encoding.UTF8.GetString(byteValue),
                _ => value?.ToString()
            };

        return null;
    }
}