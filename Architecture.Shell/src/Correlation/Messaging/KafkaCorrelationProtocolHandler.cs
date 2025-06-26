using Confluent.Kafka;
using System.Text;

namespace Architecture.Shell.Correlation.Messaging;

/// <summary>
/// Protocol handler for Kafka correlation ID propagation using Headers
/// </summary>
public class KafkaCorrelationProtocolHandler : ICorrelationProtocolHandler
{
    public Type MetadataType => typeof(Headers);

    public void Inject(object metadata, string headerName, string correlationId)
    {
        if (metadata is not Headers headers)
            return;

        var correlationBytes = Encoding.UTF8.GetBytes(correlationId);
        
        // Remove existing header if present
        headers.Remove(headerName);
        
        // Add new header
        headers.Add(headerName, correlationBytes);
    }

    public string? Extract(object metadata, string headerName)
    {
        if (metadata is not Headers headers)
            return null;

        if (headers.TryGetLastBytes(headerName, out var correlationBytes))
        {
            return Encoding.UTF8.GetString(correlationBytes);
        }

        return null;
    }
}

