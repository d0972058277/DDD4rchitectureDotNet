using CorrelationId;
using CorrelationId.Abstractions;
using Microsoft.Extensions.Options;

namespace Architecture.Shell.Correlation;

public class CorrelationService : ICorrelationService
{
    private readonly ICorrelationContextFactory _correlationContextFactory;
    private readonly ICorrelationContextAccessor _correlationContextAccessor;
    private readonly CorrelationIdOptions _correlationIdOptions;

    public CorrelationService(ICorrelationContextFactory correlationContextFactory, ICorrelationContextAccessor correlationContextAccessor, IOptions<CorrelationIdOptions> correlationIdOptions)
    {
        _correlationContextFactory = correlationContextFactory;
        _correlationContextAccessor = correlationContextAccessor;
        _correlationIdOptions = correlationIdOptions.Value;
    }

    public Guid CorrelationId
    {
        get
        {
            var hadCorrelationId = Guid.TryParse(_correlationContextAccessor.CorrelationContext?.CorrelationId, out var correlationId);

            if (hadCorrelationId)
                return correlationId;

            correlationId = Guid.NewGuid();
            _correlationContextFactory.Create(correlationId.ToString(), _correlationIdOptions.RequestHeader);
            return correlationId;
        }
    }
}
