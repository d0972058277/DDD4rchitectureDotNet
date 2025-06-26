namespace Architecture.Shell.Correlation;

/// <summary>
/// Interface for propagating correlation IDs across different communication protocols
/// </summary>
public interface ICorrelationPropagator
{
    /// <summary>
    /// Injects correlation context into outgoing communication metadata
    /// </summary>
    /// <param name="metadata">Protocol-specific metadata container</param>
    void Inject<TMetadata>(TMetadata metadata) where TMetadata : class;

    /// <summary>
    /// Extracts correlation context from incoming communication metadata
    /// </summary>
    /// <param name="metadata">Protocol-specific metadata container</param>
    /// <returns>Extracted correlation ID or null if not found</returns>
    string? Extract<TMetadata>(TMetadata metadata) where TMetadata : class;
}