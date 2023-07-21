using Xunit.Extensions.Ordering;

namespace Architecture.Tests;

public class SystemDateTimeTests
{
    [Fact, Order(1)]
    public void 初始化前_應該會拋出ArgumentNullException()
    {
        // Given

        // When
        var func = () => SystemDateTime.UtcNow;

        // Then
        func.Should().Throw<ArgumentNullException>();
    }

    [Fact, Order(2)]
    public void 初始化後_應該透過初始化的設定內容取得DateTime()
    {
        // Given
        var dateTime = new DateTime(2023, 1, 1);
        var func = () => dateTime;
        SystemDateTime.InitUtcNow(func);

        // When
        var now = SystemDateTime.UtcNow;

        // Then
        now.Should().Be(dateTime);
    }
}
