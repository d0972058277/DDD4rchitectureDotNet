using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Architecture.Shell.Correlation;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICorrelationContextFactory _correlationContextFactory;
    private readonly CorrelationIdOptions _options;
    private readonly ILogger<CorrelationMiddleware> _logger;

    public CorrelationMiddleware(
        RequestDelegate next,
        ICorrelationContextFactory correlationContextFactory,
        IOptions<CorrelationIdOptions> options,
        ILogger<CorrelationMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _correlationContextFactory = correlationContextFactory ?? throw new ArgumentNullException(nameof(correlationContextFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        if (string.IsNullOrEmpty(correlationId))
        {
            if (_options.EnforceHeader)
            {
                _logger.LogWarning("Correlation header {Header} is required but not provided", _options.RequestHeader);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Header '{_options.RequestHeader}' is required.");
                return;
            }

            correlationId = GenerateCorrelationId();
            _logger.LogDebug("Generated new correlation ID: {CorrelationId}", correlationId);
        }
        else
        {
            _logger.LogDebug("Using correlation ID from request: {CorrelationId}", correlationId);
        }

        var correlationContext = _correlationContextFactory.Create(correlationId, _options.RequestHeader);

        try
        {
            if (_options.UpdateTraceIdentifier)
            {
                context.TraceIdentifier = correlationId;
            }

            if (_options.AddToLoggingScope)
            {
                using var scope = _logger.BeginScope(new Dictionary<string, object>
                {
                    [_options.LoggingScopeKey] = correlationId
                });

                await ProcessRequestAsync(context, correlationId);
            }
            else
            {
                await ProcessRequestAsync(context, correlationId);
            }
        }
        finally
        {
            _correlationContextFactory.Dispose();
        }
    }

    private async Task ProcessRequestAsync(HttpContext context, string correlationId)
    {
        if (_options.IncludeInResponse)
        {
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(_options.ResponseHeader))
                {
                    context.Response.Headers.Add(_options.ResponseHeader, correlationId);
                }
                return Task.CompletedTask;
            });
        }

        await _next(context);
    }

    private string GetCorrelationId(HttpContext context)
    {
        if (_options.IgnoreRequestHeader)
        {
            return string.Empty;
        }

        return context.Request.Headers[_options.RequestHeader].FirstOrDefault() ?? string.Empty;
    }

    private string GenerateCorrelationId()
    {
        if (_options.CorrelationIdGenerator != null)
        {
            return _options.CorrelationIdGenerator();
        }

        return Guid.NewGuid().ToString();
    }
}