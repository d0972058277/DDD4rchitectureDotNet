using Architecture.Shell.Correlation;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace Architecture.Shell.Test.Correlation;

public class CorrelationServiceTests
{
    [Fact]
    public void CorrelationId_WhenContextExists_ShouldReturnExistingId()
    {
        // Given
        var existingId = Guid.NewGuid();
        var context = new CorrelationContext(existingId.ToString(), "X-Correlation-ID");
        
        var mockFactory = new Mock<ICorrelationContextFactory>();
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        
        var options = Options.Create(new CorrelationIdOptions());
        var service = new CorrelationService(mockFactory.Object, mockAccessor.Object, options);

        // When
        var result = service.CorrelationId;

        // Then
        result.ShouldBe(existingId);
        mockFactory.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void CorrelationId_WhenContextIsNull_ShouldCreateNewId()
    {
        // Given
        var mockFactory = new Mock<ICorrelationContextFactory>();
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        mockAccessor.Setup(x => x.CorrelationContext).Returns((CorrelationContext)null);
        
        var options = Options.Create(new CorrelationIdOptions { RequestHeader = "X-Custom-Header" });
        var service = new CorrelationService(mockFactory.Object, mockAccessor.Object, options);

        // When
        var result = service.CorrelationId;

        // Then
        result.ShouldNotBe(Guid.Empty);
        mockFactory.Verify(x => x.Create(result.ToString(), "X-Custom-Header"), Times.Once);
    }

    [Fact]
    public void CorrelationId_WhenContextHasInvalidId_ShouldCreateNewId()
    {
        // Given
        var context = new CorrelationContext("invalid-guid", "X-Correlation-ID");
        
        var mockFactory = new Mock<ICorrelationContextFactory>();
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        
        var options = Options.Create(new CorrelationIdOptions());
        var service = new CorrelationService(mockFactory.Object, mockAccessor.Object, options);

        // When
        var result = service.CorrelationId;

        // Then
        result.ShouldNotBe(Guid.Empty);
        mockFactory.Verify(x => x.Create(result.ToString(), CorrelationIdOptions.DefaultHeader), Times.Once);
    }

    [Fact]
    public void CorrelationId_WhenContextHasDefaultValue_ShouldCreateNewId()
    {
        // Given
        var context = new CorrelationContext(CorrelationContext.DefaultCorrelationId, "X-Correlation-ID");
        
        var mockFactory = new Mock<ICorrelationContextFactory>();
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        
        var options = Options.Create(new CorrelationIdOptions());
        var service = new CorrelationService(mockFactory.Object, mockAccessor.Object, options);

        // When
        var result = service.CorrelationId;

        // Then
        result.ShouldNotBe(Guid.Empty);
        mockFactory.Verify(x => x.Create(result.ToString(), CorrelationIdOptions.DefaultHeader), Times.Once);
    }

    [Fact]
    public void CorrelationId_MultipleCalls_WhenContextExists_ShouldReturnSameId()
    {
        // Given
        var existingId = Guid.NewGuid();
        var context = new CorrelationContext(existingId.ToString(), "X-Correlation-ID");
        
        var mockFactory = new Mock<ICorrelationContextFactory>();
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        
        var options = Options.Create(new CorrelationIdOptions());
        var service = new CorrelationService(mockFactory.Object, mockAccessor.Object, options);

        // When
        var result1 = service.CorrelationId;
        var result2 = service.CorrelationId;

        // Then
        result1.ShouldBe(result2);
        result1.ShouldBe(existingId);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldNotThrow()
    {
        // Given
        var mockFactory = new Mock<ICorrelationContextFactory>();
        var mockAccessor = new Mock<ICorrelationContextAccessor>();
        var options = Options.Create(new CorrelationIdOptions());

        // When & Assert
        Should.NotThrow(() => new CorrelationService(mockFactory.Object, mockAccessor.Object, options));
    }
}