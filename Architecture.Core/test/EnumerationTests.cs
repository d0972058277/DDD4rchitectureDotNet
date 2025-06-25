using Shouldly;

namespace Architecture.Core.Test;

public class EnumerationTests
{
    [Fact]
    public void GetAll_應該回傳所有的枚舉值()
    {
        // When
        var allColors = Enumeration.GetAll<TestColor>();

        // Then
        allColors.ShouldNotBeEmpty();
        allColors.Count().ShouldBe(3);
        allColors.ShouldContain(TestColor.Red);
        allColors.ShouldContain(TestColor.Green);
        allColors.ShouldContain(TestColor.Blue);
    }

    [Fact]
    public void FromValue_應該根據Id回傳對應的枚舉值()
    {
        // When
        var color = Enumeration.FromValue<TestColor>(1);

        // Then
        color.ShouldBe(TestColor.Red);
    }

    [Fact]
    public void FromValue_當Id不存在時_應該拋出異常()
    {
        // When & Then
        Should.Throw<InvalidOperationException>(() => Enumeration.FromValue<TestColor>(999));
    }

    [Fact]
    public void FromDisplayName_應該根據名稱回傳對應的枚舉值()
    {
        // When
        var color = Enumeration.FromDisplayName<TestColor>("Red");

        // Then
        color.ShouldBe(TestColor.Red);
    }

    [Fact]
    public void FromDisplayName_當名稱不存在時_應該拋出異常()
    {
        // When & Then
        Should.Throw<InvalidOperationException>(() => Enumeration.FromDisplayName<TestColor>("Purple"));
    }

    [Fact]
    public void Equals_當類型和Id相同時_應該回傳true()
    {
        // Given
        var color1 = TestColor.Red;
        var color2 = TestColor.Red;

        // When & Then
        color1.Equals(color2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_當類型不同時_應該回傳false()
    {
        // Given
        var color = TestColor.Red;
        var size = TestSize.Small;

        // When & Then
        color.Equals(size).ShouldBeFalse();
    }

    [Fact]
    public void AbsoluteDifference_應該回傳兩個枚舉值的絕對差值()
    {
        // When
        var difference = Enumeration.AbsoluteDifference(TestColor.Red, TestColor.Blue);

        // Then
        difference.ShouldBe(2);
    }

    [Fact]
    public void CompareTo_應該根據Id進行比較()
    {
        // Given
        var red = TestColor.Red;
        var blue = TestColor.Blue;

        // When & Then
        red.CompareTo(blue).ShouldBeLessThan(0);
        blue.CompareTo(red).ShouldBeGreaterThan(0);
        red.CompareTo(red).ShouldBe(0);
    }

    [Fact]
    public void ToString_應該回傳名稱()
    {
        // When & Then
        TestColor.Red.ToString().ShouldBe("Red");
    }

    [Fact]
    public void GetHashCode_應該回傳Id的HashCode()
    {
        // When & Then
        TestColor.Red.GetHashCode().ShouldBe(1.GetHashCode());
    }

    public class TestColor(int id, string name) : Enumeration(id, name)
    {
        public static TestColor Red = new(1, "Red");
        public static TestColor Green = new(2, "Green");
        public static TestColor Blue = new(3, "Blue");
    }

    public class TestSize(int id, string name) : Enumeration(id, name)
    {
        public static TestSize Small = new(1, "Small");
        public static TestSize Medium = new(2, "Medium");
        public static TestSize Large = new(3, "Large");
    }
}