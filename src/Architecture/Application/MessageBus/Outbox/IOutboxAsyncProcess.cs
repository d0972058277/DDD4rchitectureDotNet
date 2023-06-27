namespace Architecture.Application.MessageBus.Outbox
{
    public interface IOutboxAsyncProcess
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}