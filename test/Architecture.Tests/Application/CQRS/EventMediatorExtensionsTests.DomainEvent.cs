using Architecture.Domain;

namespace Architecture.Tests.Application.CQRS
{
    public partial class EventMediatorExtensionsTests
    {
        public class DomainEventA : IDomainEvent { };
        public class DomainEventB : IDomainEvent { };
        public class DomainEventC : IDomainEvent { };
    }
}