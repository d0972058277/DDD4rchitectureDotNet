namespace Architecture.Application.MessageBus.Outbox
{
    public interface IOutboxProcessor
    {
        Task PublishAsync(CancellationToken cancellationToken = default);
    }
}