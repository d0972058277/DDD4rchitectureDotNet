using System;
using System.Linq;
using System.Net.Http.Headers;

namespace Architecture.Shell.Correlation.Http;

/// <summary>
/// HTTP-specific correlation ID propagation handler
/// </summary>
public class HttpCorrelationProtocolHandler : ICorrelationProtocolHandler
{
    public Type MetadataType => typeof(HttpHeaders);

    public void Inject(object metadata, string headerName, string correlationId)
    {
        if (metadata is HttpHeaders headers)
        {
            // Remove existing correlation header if present
            headers.Remove(headerName);

            // Add correlation ID header
            headers.Add(headerName, correlationId);
        }
    }

    public string? Extract(object metadata, string headerName)
    {
        if (metadata is HttpHeaders headers)
            if (headers.TryGetValues(headerName, out var values))
                return values.FirstOrDefault();

        return null;
    }
}