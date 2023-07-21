using SequentialGuid;

namespace Architecture
{
    public static class IdGenerator
    {
        public static Guid NewId() => SequentialGuidGenerator.Instance.NewGuid();
    }
}