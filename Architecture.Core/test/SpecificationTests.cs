using Shouldly;

namespace Architecture.Core.Test;

public class SpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_當所有規則都通過時_應該回傳成功結果()
    {
        // Given
        var entity = new TestEntity { Name = "Valid", Age = 25 };
        var specification = new TestSpecification();

        // When
        var result = specification.IsSatisfiedBy(entity);

        // Then
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(entity);
    }

    [Fact]
    public void IsSatisfiedBy_當有規則失敗時_應該回傳失敗結果()
    {
        // Given
        var entity = new TestEntity { Name = "", Age = 25 };
        var specification = new TestSpecification();

        // When
        var result = specification.IsSatisfiedBy(entity);

        // Then
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Name cannot be empty");
    }

    [Fact]
    public void GetRules_應該回傳所有規則()
    {
        // Given
        var specification = new TestSpecification();

        // When
        var rules = specification.GetRules();

        // Then
        rules.Count.ShouldBe(2);
    }

    private class TestEntity
    {
        public string Name { get; init; } = string.Empty;
        public int Age { get; init; }
    }

    private class TestSpecification : SpecificationBase<TestEntity>
    {
        protected override IEnumerable<SpecificationRule<TestEntity>> GetSpecificationRules()
        {
            yield return new TestSpecificationRule("Name cannot be empty", x => !string.IsNullOrEmpty(x.Name));
            yield return new TestSpecificationRule("Age must be positive", x => x.Age > 0);
        }
    }

    private class TestSpecificationRule(string message, Func<TestEntity, bool> validate)
        : SpecificationRule<TestEntity>(message, validate);
}