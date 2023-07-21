using Architecture.Application.MessageBus;

namespace Architecture.Tests.Application.MessageBus.Outbox
{
    public partial class TransactionalEventBusTests
    {
        public class SomethingIntegrationEvent : IntegrationEvent { };
    }
}