using System.Collections.Concurrent;
using System.Reflection;
using Architecture.Application.CQRS;
using Architecture.Domain;
using BenchmarkDotNet.Attributes;
using Moq;

namespace Architecture.Benchmark
{
    public class DomainEvent0 : IDomainEvent { }
    public class DomainEvent1 : IDomainEvent { }
    public class DomainEvent2 : IDomainEvent { }
    public class DomainEvent3 : IDomainEvent { }
    public class DomainEvent4 : IDomainEvent { }
    public class DomainEvent5 : IDomainEvent { }
    public class DomainEvent6 : IDomainEvent { }
    public class DomainEvent7 : IDomainEvent { }
    public class DomainEvent8 : IDomainEvent { }
    public class DomainEvent9 : IDomainEvent { }

    [MemoryDiagnoser]
    public class EventMediatorPublishEvnets
    {
        public static IReadOnlyList<IDomainEvent> GetDomainEvents()
        {
            return new List<IDomainEvent>
            {
                new DomainEvent0(),
                new DomainEvent1(),
                new DomainEvent2(),
                new DomainEvent3(),
                new DomainEvent4(),
                new DomainEvent5(),
                new DomainEvent6(),
                new DomainEvent7(),
                new DomainEvent8(),
                new DomainEvent9()
            };
        }

        [Benchmark]
        public async Task 不特別找出範型方法()
        {
            var mediator = new Mock<IEventMediator>();
            var events = GetDomainEvents();

            foreach (var e in events)
            {
                await mediator.Object.PublishAsync(e);
            }
        }

        [Benchmark]
        public async Task 特別找出範型方法()
        {
            var mediator = new Mock<IEventMediator>();
            var events = GetDomainEvents();

            foreach (var e in events)
            {
                var domainEventType = e.GetType();
                var publishMethod = typeof(IEventMediator)
                    .GetMethod(nameof(IEventMediator.PublishAsync))!
                    .MakeGenericMethod(domainEventType);

                await (Task)publishMethod.Invoke(mediator.Object, new object[] { e, default(CancellationToken) })!;
            }
        }

        private static readonly ConcurrentDictionary<Type, MethodInfo> PublishMethodsCache = new();

        [Benchmark]
        public async Task 特別找出範型方法_且快取方法()
        {
            var mediator = new Mock<IEventMediator>();
            var events = GetDomainEvents();

            foreach (var e in events)
            {
                var domainEventType = e.GetType();
                var publishMethod = PublishMethodsCache.GetOrAdd(domainEventType, GetPublishMethod);
                await (Task)publishMethod.Invoke(mediator.Object, new object[] { e, default(CancellationToken) })!;
            }
        }

        private static MethodInfo GetPublishMethod(Type domainEventType)
        {
            var publishMethod = typeof(IEventMediator)
                .GetMethod(nameof(IEventMediator.PublishAsync))!
                .MakeGenericMethod(domainEventType);
            return publishMethod;
        }
    }
}