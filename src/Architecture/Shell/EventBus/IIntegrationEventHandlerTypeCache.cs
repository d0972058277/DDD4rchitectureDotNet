namespace Architecture.Shell.EventBus;

public interface IIntegrationEventHandlerTypeCache
{
    IReadOnlyList<Type> GetImpHandlerTypes(Type integrationEventType);
}
