using System;

namespace Architecture.Shell.Correlation;

/// <summary>
/// Protocol-specific handler for correlation ID propagation
/// </summary>
public interface ICorrelationProtocolHandler
{
    /// <summary>
    /// The metadata type this handler supports
    /// </summary>
    Type MetadataType { get; }

    /// <summary>
    /// Injects correlation ID into protocol-specific metadata
    /// </summary>
    void Inject(object metadata, string headerName, string correlationId);

    /// <summary>
    /// Extracts correlation ID from protocol-specific metadata
    /// </summary>
    string? Extract(object metadata, string headerName);
}