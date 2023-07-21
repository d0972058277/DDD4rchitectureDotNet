using Architecture;
using FluentAssertions;
using Project.Domain.SomethingContext.Models;

namespace Project.Unit.Tests.SomethingContext.Domain;

public class SomethingEntityTests
{
    [Fact]
    public void 帶入正確的參數_應該能夠成功建立()
    {
        // Given
        var id = IdGenerator.NewId();
        var name = "name";

        // When
        var entity = SomethingEntity.Create(id, name);

        // Then
        entity.Id.Should().Be(id);
        entity.Name.Should().Be(name);
    }

    [Fact]
    public void 應該能夠重新命名()
    {
        // Given
        var entity = GetSomethingEntity();
        var newName = "newName";

        // When
        entity.Rename(newName);

        // Then
        entity.Name.Should().Be(newName);
    }

    private static SomethingEntity GetSomethingEntity()
    {
        var id = IdGenerator.NewId();
        var name = "name";
        var entity = SomethingEntity.Create(id, name);
        return entity;
    }
}
