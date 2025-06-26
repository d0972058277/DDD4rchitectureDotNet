namespace Architecture.Shell.Correlation.Http;

/// <summary>
/// HTTP message handler that automatically injects correlation IDs into outgoing HTTP requests
/// </summary>
public class HttpCorrelationHandler : DelegatingHandler
{
    private readonly ICorrelationPropagator _correlationPropagator;

    public HttpCorrelationHandler(ICorrelationPropagator correlationPropagator)
    {
        _correlationPropagator = correlationPropagator ?? throw new ArgumentNullException(nameof(correlationPropagator));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Inject correlation ID into request headers
        _correlationPropagator.Inject(request.Headers);

        return base.SendAsync(request, cancellationToken);
    }
}