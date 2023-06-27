namespace Architecture.Application.MessageBus
{
    public abstract class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = IdGenerator.NewId();
            CreationTimestamp = DateTime.UtcNow;
        }

        public Guid Id { get; }

        public DateTime CreationTimestamp { get; }
    }
}