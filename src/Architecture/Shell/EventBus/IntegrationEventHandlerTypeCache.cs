using System.Collections.Concurrent;

namespace Architecture.Shell.EventBus;

public class IntegrationEventHandlerTypeCache : IIntegrationEventHandlerTypeCache
{
    private readonly ConcurrentDictionary<Type, IReadOnlyList<Type>> _handlerCache = new();

    public IReadOnlyList<Type> GetImpHandlerTypes(Type integrationEventType)
    {
        return _handlerCache.GetOrAdd(integrationEventType, integrationEventType =>
        {
            var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(integrationEventType);
            var handlerImplementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && handlerType.IsAssignableFrom(type))
                .ToList();

            return handlerImplementations;
        });
    }
}