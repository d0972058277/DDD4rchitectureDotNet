using System;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Architecture.Shell.Correlation;

/// <summary>
/// Default implementation of correlation propagator that supports multiple protocols
/// </summary>
public class CorrelationPropagator : ICorrelationPropagator
{
    private readonly ICorrelationContextAccessor _correlationContextAccessor;
    private readonly CorrelationIdOptions _options;
    private readonly Dictionary<Type, ICorrelationProtocolHandler> _protocolHandlers;

    public CorrelationPropagator(
        ICorrelationContextAccessor correlationContextAccessor,
        IOptions<CorrelationIdOptions> options,
        IEnumerable<ICorrelationProtocolHandler> protocolHandlers)
    {
        _correlationContextAccessor = correlationContextAccessor ??
                                      throw new ArgumentNullException(nameof(correlationContextAccessor));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _protocolHandlers = protocolHandlers?.ToDictionary(h => h.MetadataType, h => h) ??
                            throw new ArgumentNullException(nameof(protocolHandlers));
    }

    public void Inject<TMetadata>(TMetadata metadata) where TMetadata : class
    {
        var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId;
        if (string.IsNullOrEmpty(correlationId) || correlationId == CorrelationContext.DefaultCorrelationId)
            return;

        if (_protocolHandlers.TryGetValue(typeof(TMetadata), out var handler))
            handler.Inject(metadata, _options.RequestHeader, correlationId);
        else
            // Fallback for generic metadata containers
            InjectGeneric(metadata, correlationId);
    }

    public string? Extract<TMetadata>(TMetadata metadata) where TMetadata : class
    {
        if (_protocolHandlers.TryGetValue(typeof(TMetadata), out var handler))
            return handler.Extract(metadata, _options.RequestHeader);

        // Fallback for generic metadata containers
        return ExtractGeneric(metadata);
    }

    private void InjectGeneric<TMetadata>(TMetadata metadata, string correlationId) where TMetadata : class
    {
        // Support for IDictionary-based metadata (common in many RPC frameworks)
        if (metadata is IDictionary<string, string> stringDict)
            stringDict[_options.RequestHeader] = correlationId;
        else if (metadata is IDictionary<string, object> objectDict)
            objectDict[_options.RequestHeader] = correlationId;
        else if (metadata is IDictionary dict) dict[_options.RequestHeader] = correlationId;
    }

    private string? ExtractGeneric<TMetadata>(TMetadata metadata) where TMetadata : class
    {
        // Support for IDictionary-based metadata
        if (metadata is IDictionary<string, string> stringDict)
            return stringDict.TryGetValue(_options.RequestHeader, out var value) ? value : null;
        else if (metadata is IDictionary<string, object> objectDict)
            return objectDict.TryGetValue(_options.RequestHeader, out var value) ? value?.ToString() : null;
        else if (metadata is IDictionary dict) return dict[_options.RequestHeader]?.ToString();

        return null;
    }
}