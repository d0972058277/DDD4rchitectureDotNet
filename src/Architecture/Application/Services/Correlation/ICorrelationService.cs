namespace Architecture.Application.Services.Correlation;

public interface ICorrelationService : IApplicationService
{
    Guid CorrelationId { get; }
}
