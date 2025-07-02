using Architecture.Shell.Correlation;
using Shouldly;

namespace Architecture.Shell.Correlation.Test;

public class CorrelationContextTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateContext()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var header = "X-Correlation-ID";

        // When
        var context = new CorrelationContext(correlationId, header);

        // Then
        context.CorrelationId.ShouldBe(correlationId);
        context.Header.ShouldBe(header);
    }

    [Fact]
    public void Constructor_WithNullCorrelationId_ShouldUseDefaultValue()
    {
        // Given
        string correlationId = null;
        var header = "X-Correlation-ID";

        // When
        var context = new CorrelationContext(correlationId, header);

        // Then
        context.CorrelationId.ShouldBe(CorrelationContext.DefaultCorrelationId);
        context.Header.ShouldBe(header);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidHeader_ShouldThrowArgumentException(string header)
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();

        // When & Assert
        var exception = Should.Throw<ArgumentException>(() => new CorrelationContext(correlationId, header));
        exception.ParamName.ShouldBe("header");
        exception.Message.ShouldContain("A header must be provided.");
    }

    [Fact]
    public void DefaultCorrelationId_ShouldHaveExpectedValue()
    {
        // Then
        CorrelationContext.DefaultCorrelationId.ShouldBe("Not set");
    }
}