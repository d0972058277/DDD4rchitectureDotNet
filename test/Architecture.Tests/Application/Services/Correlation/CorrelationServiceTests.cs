using Architecture.Application.Services.Correlation;
using CorrelationId;
using CorrelationId.Abstractions;
using Microsoft.Extensions.Options;

namespace Architecture.Tests.Application.Services.Correlation;

public class CorrelationServiceTests
{
    [Fact]
    public void 假如已有CorrelationId_應該直接回傳CorrelationId()
    {
        // Given
        var correlationContext = new CorrelationContext(Guid.NewGuid().ToString(), "x-correlation-id");

        var correlationContextFactory = new Mock<ICorrelationContextFactory>();
        var correlationContextAccessor = new Mock<ICorrelationContextAccessor>();
        correlationContextAccessor.Setup(m => m.CorrelationContext).Returns(correlationContext);
        var correlationIdOptions = new CorrelationIdOptions();

        var service = new CorrelationService(correlationContextFactory.Object, correlationContextAccessor.Object, Options.Create(correlationIdOptions));

        // When
        var correlationId = service.CorrelationId;

        // Then
        correlationId.Should().Be(correlationContext.CorrelationId);
    }

    [Fact]
    public void 假如沒有CorrelationId_應該創建CorrelationId並回傳()
    {
        // Given
        var correlationContextFactory = new Mock<ICorrelationContextFactory>();
        var correlationContextAccessor = new Mock<ICorrelationContextAccessor>();
        var correlationIdOptions = new CorrelationIdOptions();

        var service = new CorrelationService(correlationContextFactory.Object, correlationContextAccessor.Object, Options.Create(correlationIdOptions));

        // When
        var correlationId = service.CorrelationId;

        // Then
        correlationContextFactory.Verify(m => m.Create(correlationId.ToString(), correlationIdOptions.RequestHeader), Times.Once());
        correlationId.Should().NotBeEmpty();
    }
}
