using Shouldly;

namespace Architecture.Core.Test;

public class SelectorTests
{
    [Fact]
    public void Set_應該建立選擇器並回傳屬性值()
    {
        // Given
        var entity = new TestEntity { Name = "Test", Age = 25 };
        var selector = Selector<TestEntity>.Set(x => x.Name);

        // When
        var value = selector.GetValue(entity);

        // Then
        value.ShouldBe("Test");
        selector.PropertyName.ShouldBe("Name");
    }

    [Fact]
    public void Set_使用工廠方法_應該建立選擇器並轉換值()
    {
        // Given
        var entity = new TestEntity { Name = "Test", Age = 25 };
        var selector = Selector<TestEntity>.Set(x => x.Age, age => age.ToString());

        // When
        var value = selector.GetValue(entity);

        // Then
        value.ShouldBe("25");
        selector.PropertyName.ShouldBe("Age");
    }

    private class TestEntity
    {
        public string Name { get; init; } = string.Empty;
        public int Age { get; init; }
    }
}