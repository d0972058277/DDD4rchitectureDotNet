using Architecture;
using FluentAssertions;
using Project.Domain.SomethingContext.Models;

namespace Project.Unit.Tests.SomethingContext.Domain;

public class SomethingValueObjectTests
{
    [Fact]
    public void 帶入正確的參數_應該能夠成功建立()
    {
        // Given
        var @string = "string";
        var number = 1;
        var boolean = true;
        var dateTime = SystemDateTime.UtcNow;

        // When
        var result = SomethingValueObject.Create(@string, number, boolean, dateTime);

        // Then
        result.IsSuccess.Should().BeTrue();
        result.Value.String.Should().Be(@string);
        result.Value.Number.Should().Be(number);
        result.Value.Boolean.Should().Be(boolean);
        result.Value.DateTime.Should().Be(dateTime);
    }

    [Theory]
    [InlineData(default(string))]
    [InlineData("")]
    public void 建立時_String應該不可為Null或空字串(string @string)
    {
        // Given
        var number = 1;
        var boolean = true;
        var dateTime = SystemDateTime.UtcNow;

        // When
        var result = SomethingValueObject.Create(@string, number, boolean, dateTime);

        // Then
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void 建立時_Number應該不可小於0()
    {
        // Given
        var @string = "string";
        var number = -1;
        var boolean = true;
        var dateTime = SystemDateTime.UtcNow;

        // When
        var result = SomethingValueObject.Create(@string, number, boolean, dateTime);

        // Then
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void 建立時_Boolean應該不可為False()
    {
        // Given
        var @string = "string";
        var number = 1;
        var boolean = false;
        var dateTime = SystemDateTime.UtcNow;

        // When
        var result = SomethingValueObject.Create(@string, number, boolean, dateTime);

        // Then
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void 建立時_DateTime應該不可小於系統的現在時間()
    {
        // Given
        var @string = "string";
        var number = 1;
        var boolean = false;
        var dateTime = SystemDateTime.UtcNow.AddSeconds(-1);

        // When
        var result = SomethingValueObject.Create(@string, number, boolean, dateTime);

        // Then
        result.IsFailure.Should().BeTrue();
    }
}
