using Architecture.Shell.Correlation;
using Moq;
using Shouldly;

namespace Architecture.Shell.Correlation.Test;

public class CorrelationContextFactoryTests
{
    [Fact]
    public void Create_WithoutAccessor_ShouldCreateContext()
    {
        // Given
        var factory = new CorrelationContextFactory();
        var correlationId = Guid.NewGuid().ToString();
        var header = "X-Correlation-ID";

        // When
        var context = factory.Create(correlationId, header);

        // Then
        context.ShouldNotBeNull();
        context.CorrelationId.ShouldBe(correlationId);
        context.Header.ShouldBe(header);
    }

    [Fact]
    public void Create_WithAccessor_ShouldCreateContextAndSetInAccessor()
    {
        // Given
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        var factory = new CorrelationContextFactory(mockAccessor.Object);
        var correlationId = Guid.NewGuid().ToString();
        var header = "X-Correlation-ID";

        // When
        var context = factory.Create(correlationId, header);

        // Then
        context.ShouldNotBeNull();
        context.CorrelationId.ShouldBe(correlationId);
        context.Header.ShouldBe(header);
        mockAccessor.VerifySet(x => x.CorrelationContext = context, Times.Once);
    }

    [Fact]
    public void Create_WithNullAccessor_ShouldNotThrow()
    {
        // Given
        var factory = new CorrelationContextFactory(null);
        var correlationId = Guid.NewGuid().ToString();
        var header = "X-Correlation-ID";

        // When & Assert
        Should.NotThrow(() => factory.Create(correlationId, header));
    }

    [Fact]
    public void Dispose_WithoutAccessor_ShouldNotThrow()
    {
        // Given
        var factory = new CorrelationContextFactory();

        // When & Assert
        Should.NotThrow(() => factory.Dispose());
    }

    [Fact]
    public void Dispose_WithAccessor_ShouldClearContext()
    {
        // Given
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        var factory = new CorrelationContextFactory(mockAccessor.Object);

        // When
        factory.Dispose();

        // Then
        mockAccessor.VerifySet(x => x.CorrelationContext = null, Times.Once);
    }

    [Fact]
    public void Dispose_WithNullAccessor_ShouldNotThrow()
    {
        // Given
        var factory = new CorrelationContextFactory(null);

        // When & Assert
        Should.NotThrow(() => factory.Dispose());
    }

    [Fact]
    public void DefaultConstructor_ShouldInitializeWithNullAccessor()
    {
        // When & Assert
        Should.NotThrow(() => new CorrelationContextFactory());
    }
}