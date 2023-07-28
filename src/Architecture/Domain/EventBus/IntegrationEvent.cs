using System.Text.Json;
using CSharpFunctionalExtensions;

namespace Architecture.Domain.EventBus;

public class IntegrationEvent : ValueObject
{
    private IntegrationEvent(Guid id, DateTime creationTimestamp, string typeName, string message)
    {
        Id = id;
        CreationTimestamp = creationTimestamp;
        TypeName = typeName;
        Message = message;
    }

    public Guid Id { get; }
    public DateTime CreationTimestamp { get; }
    public string TypeName { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Id;
        yield return CreationTimestamp;
        yield return TypeName;
        yield return Message;
    }

    public static IntegrationEvent Create<T>(T integrationEvent) where T : IIntegrationEvent
    {
        var typeName = integrationEvent.GetType().FullName;
        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("無法取得 IntegrationEvent 參數的 FullName ，無法建立。");

        var id = integrationEvent.Id;
        var creationTimestamp = integrationEvent.CreationTimestamp;
        var message = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType());

        return new IntegrationEvent(id, creationTimestamp, typeName, message);
    }

    internal static IntegrationEvent Create(Guid id, DateTime creationTimestamp, string typeName, string message)
    {
        return new IntegrationEvent(id, creationTimestamp, typeName, message);
    }
}
