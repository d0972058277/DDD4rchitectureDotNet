namespace Architecture.Shell.Correlation;

public interface ICorrelationService : IApplicationService
{
    Guid CorrelationId { get; }
}
