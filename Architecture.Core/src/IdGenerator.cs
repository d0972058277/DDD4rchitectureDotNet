using SequentialGuid;

namespace Architecture.Core
{
    public static class IdGenerator
    {
        public static Guid NewId() => SequentialGuidGenerator.Instance.NewGuid();
    }
}