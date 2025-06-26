#if !NETSTANDARD2_0
using Grpc.Core;

namespace Architecture.Shell.Correlation.Grpc;

/// <summary>
/// gRPC-specific correlation ID propagation handler
/// </summary>
public class GrpcCorrelationProtocolHandler : ICorrelationProtocolHandler
{
    public Type MetadataType => typeof(Metadata);

    public void Inject(object metadata, string headerName, string correlationId)
    {
        if (metadata is Metadata grpcMetadata)
        {
            // Remove existing correlation header if present
            var existingEntry = grpcMetadata.FirstOrDefault(e => 
                string.Equals(e.Key, headerName, StringComparison.OrdinalIgnoreCase));
            
            if (existingEntry != null)
            {
                grpcMetadata.Remove(existingEntry);
            }

            // Add correlation ID header
            grpcMetadata.Add(headerName.ToLowerInvariant(), correlationId);
        }
    }

    public string? Extract(object metadata, string headerName)
    {
        if (metadata is Metadata grpcMetadata)
        {
            var entry = grpcMetadata.FirstOrDefault(e => 
                string.Equals(e.Key, headerName, StringComparison.OrdinalIgnoreCase));
            
            return entry?.Value;
        }

        return null;
    }
}
#endif