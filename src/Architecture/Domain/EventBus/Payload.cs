using System.Text.Json;
using CSharpFunctionalExtensions;

namespace Architecture.Domain.EventBus;

public class Payload : ValueObject
{
    private Payload(Guid id, DateTime creationTimestamp, string typeName, string content)
    {
        Id = id;
        CreationTimestamp = creationTimestamp;
        TypeName = typeName;
        Content = content;
    }

    public Guid Id { get; }
    public DateTime CreationTimestamp { get; }
    public string TypeName { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Id;
        yield return CreationTimestamp;
        yield return TypeName;
        yield return Content;
    }

    public static Payload Serialize<T>(T integrationEvent) where T : IIntegrationEvent
    {
        var typeName = integrationEvent.GetType().FullName;
        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("無法取得 IntegrationEvent 參數的 FullName ，無法序列化。");

        var id = integrationEvent.Id;
        var creationTimestamp = integrationEvent.CreationTimestamp;
        var content = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType());

        return new Payload(id, creationTimestamp, typeName, content);
    }

    internal static Payload Create(Guid id, DateTime creationTimestamp, string typeName, string content)
    {
        return new Payload(id, creationTimestamp, typeName, content);
    }

    public IIntegrationEvent Deserialize()
    {
        // TODO: 反射會有效能上的問題，這邊需要優化效能
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetLoadableTypes())
            .Where(t => typeof(IIntegrationEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .ToList()
            .Find(t => t.FullName == TypeName)!;

        var integrationEvent = (JsonSerializer.Deserialize(Content, type) as IIntegrationEvent)!;
        return integrationEvent;
    }
}
