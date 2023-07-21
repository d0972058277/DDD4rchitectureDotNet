using CSharpFunctionalExtensions;

namespace Project.Domain.SomethingContext.Models;

public class SomethingEntity : Entity<Guid>
{
    private SomethingEntity(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public static SomethingEntity Create(Guid id, string name)
    {
        return new SomethingEntity(id, name);
    }

    public void Rename(string name)
    {
        Name = name;
    }
}
