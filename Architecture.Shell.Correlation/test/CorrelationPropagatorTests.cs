using Architecture.Shell.Correlation;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace Architecture.Shell.Correlation.Test;

public class CorrelationPropagatorTests
{
    private readonly Mock<ICorrelationContextAccessor> _mockAccessor;
    private readonly Mock<ICorrelationProtocolHandler> _mockProtocolHandler;
    private readonly CorrelationIdOptions _options;
    private readonly CorrelationPropagator _propagator;

    public CorrelationPropagatorTests()
    {
        _mockAccessor = new Mock<ICorrelationContextAccessor>();
        _mockProtocolHandler = new Mock<ICorrelationProtocolHandler>();
        _options = new CorrelationIdOptions { RequestHeader = "X-Correlation-ID" };
        
        _mockProtocolHandler.Setup(x => x.MetadataType).Returns(typeof(TestMetadata));
        
        _propagator = new CorrelationPropagator(
            _mockAccessor.Object,
            Options.Create(_options),
            new[] { _mockProtocolHandler.Object });
    }

    [Fact]
    public void Inject_WithSpecificProtocolHandler_ShouldUseHandler()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var context = new CorrelationContext(correlationId, "X-Correlation-ID");
        _mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        
        var metadata = new TestMetadata();

        // When
        _propagator.Inject(metadata);

        // Then
        _mockProtocolHandler.Verify(x => x.Inject(metadata, _options.RequestHeader, correlationId), Times.Once);
    }

    [Fact]
    public void Inject_WithNoCorrelationContext_ShouldNotCallHandler()
    {
        // Given
        _mockAccessor.Setup(x => x.CorrelationContext).Returns((CorrelationContext?)null);
        var metadata = new TestMetadata();

        // When
        _propagator.Inject(metadata);

        // Then
        _mockProtocolHandler.Verify(x => x.Inject(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Inject_WithDefaultCorrelationId_ShouldNotCallHandler()
    {
        // Given
        var context = new CorrelationContext(CorrelationContext.DefaultCorrelationId, "X-Correlation-ID");
        _mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        var metadata = new TestMetadata();

        // When
        _propagator.Inject(metadata);

        // Then
        _mockProtocolHandler.Verify(x => x.Inject(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Inject_WithDictionaryMetadata_ShouldInjectIntoStringDictionary()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var context = new CorrelationContext(correlationId, "X-Correlation-ID");
        _mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        
        var metadata = new Dictionary<string, string>();

        // When
        _propagator.Inject(metadata);

        // Then
        metadata[_options.RequestHeader].ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithObjectDictionaryMetadata_ShouldInjectIntoObjectDictionary()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var context = new CorrelationContext(correlationId, "X-Correlation-ID");
        _mockAccessor.Setup(x => x.CorrelationContext).Returns(context);
        
        var metadata = new Dictionary<string, object>();

        // When
        _propagator.Inject(metadata);

        // Then
        metadata[_options.RequestHeader].ShouldBe(correlationId);
    }

    [Fact]
    public void Extract_WithSpecificProtocolHandler_ShouldUseHandler()
    {
        // Given
        var expectedCorrelationId = Guid.NewGuid().ToString();
        var metadata = new TestMetadata();
        _mockProtocolHandler.Setup(x => x.Extract(metadata, _options.RequestHeader))
                           .Returns(expectedCorrelationId);

        // When
        var result = _propagator.Extract(metadata);

        // Then
        result.ShouldBe(expectedCorrelationId);
        _mockProtocolHandler.Verify(x => x.Extract(metadata, _options.RequestHeader), Times.Once);
    }

    [Fact]
    public void Extract_WithStringDictionaryMetadata_ShouldExtractFromDictionary()
    {
        // Given
        var expectedCorrelationId = Guid.NewGuid().ToString();
        var metadata = new Dictionary<string, string>
        {
            [_options.RequestHeader] = expectedCorrelationId
        };

        // When
        var result = _propagator.Extract(metadata);

        // Then
        result.ShouldBe(expectedCorrelationId);
    }

    [Fact]
    public void Extract_WithObjectDictionaryMetadata_ShouldExtractFromDictionary()
    {
        // Given
        var expectedCorrelationId = Guid.NewGuid().ToString();
        var metadata = new Dictionary<string, object>
        {
            [_options.RequestHeader] = expectedCorrelationId
        };

        // When
        var result = _propagator.Extract(metadata);

        // Then
        result.ShouldBe(expectedCorrelationId);
    }

    [Fact]
    public void Extract_WithMissingCorrelationId_ShouldReturnNull()
    {
        // Given
        var metadata = new Dictionary<string, string>();

        // When
        var result = _propagator.Extract(metadata);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithUnknownMetadataType_ShouldReturnNull()
    {
        // Given
        var metadata = new UnknownMetadata();

        // When
        var result = _propagator.Extract(metadata);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Constructor_WithNullAccessor_ShouldThrowArgumentNullException()
    {
        // Given & When & Then
        Should.Throw<ArgumentNullException>(() => new CorrelationPropagator(
            null!,
            Options.Create(_options),
            new[] { _mockProtocolHandler.Object }))
            .ParamName.ShouldBe("correlationContextAccessor");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Given & When & Then
        Should.Throw<ArgumentNullException>(() => new CorrelationPropagator(
            _mockAccessor.Object,
            null!,
            new[] { _mockProtocolHandler.Object }))
            .ParamName.ShouldBe("options");
    }

    [Fact]
    public void Constructor_WithNullProtocolHandlers_ShouldThrowArgumentNullException()
    {
        // Given & When & Then
        Should.Throw<ArgumentNullException>(() => new CorrelationPropagator(
            _mockAccessor.Object,
            Options.Create(_options),
            null!))
            .ParamName.ShouldBe("protocolHandlers");
    }

    private class TestMetadata
    {
    }

    private class UnknownMetadata
    {
    }
}