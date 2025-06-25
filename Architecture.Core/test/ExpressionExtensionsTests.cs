using Shouldly;

namespace Architecture.Core.Test;

public abstract class ExpressionExtensionsTests
{
    [Fact]
    public void GetPropertyName_應該回傳屬性名稱()
    {
        // When
        var propertyName = ExpressionExtensions.GetPropertyName<TestEntity, string>(x => x.Name);

        // Then
        propertyName.ShouldBe("Name");
    }

    [Fact]
    public void GetPropertyName_當表達式不是MemberExpression時_應該拋出異常()
    {
        // When & Then
        Should.Throw<ArgumentException>(() =>
            ExpressionExtensions.GetPropertyName<TestEntity, string>(x => "constant"));
    }

    private abstract class TestEntity
    {
        public string Name { get; init; } = string.Empty;
        public int Age { get; init; }
    }
}