using System.Collections.Concurrent;
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
            throw new ArgumentNullException($"無法取得 IntegrationEvent {integrationEvent.GetType().Name} 參數的 FullName ，無法序列化。");

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
        var type = RuntimeTypeNameCache.Instance.GetTypeOrThrow(TypeName);
        return (JsonSerializer.Deserialize(Content, type) as IIntegrationEvent)!;
    }

    class RuntimeTypeNameCache
    {
        private static readonly Lazy<RuntimeTypeNameCache> _lazyInstance =
            new(() => new RuntimeTypeNameCache(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly ConcurrentDictionary<string, Type> _typeCache = new();

        private RuntimeTypeNameCache()
        {
            LoadTypesIntoCache();
        }

        public static RuntimeTypeNameCache Instance => _lazyInstance.Value;

        public Type GetTypeOrThrow(string typeName)
        {
            if (_typeCache.TryGetValue(typeName, out var type))
            {
                return type;
            }
            else
            {
                throw new InvalidOperationException($"Runtime 中沒有對應 {typeName} 的物件類型，無法進行 IntegrationEvent 的反序列化。");
            }
        }

        private void LoadTypesIntoCache()
        {
            var integrationEventTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(t => typeof(IIntegrationEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

            foreach (var type in integrationEventTypes)
            {
                _typeCache[type.FullName!] = type;
            }
        }
    }
}
