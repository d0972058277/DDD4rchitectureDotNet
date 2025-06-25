using SequentialGuid;

namespace Architecture.Core;

public class SequentialGuidGenerator : IIdGenerator<Guid>
{
    public Guid NewId()
    {
        return SequentialGuid.SequentialGuidGenerator.Instance.NewGuid();
    }
}