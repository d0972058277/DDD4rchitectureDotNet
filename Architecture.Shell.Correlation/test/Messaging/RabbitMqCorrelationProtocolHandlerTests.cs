using Architecture.Shell.Correlation.Messaging;
using RabbitMQ.Client;
using Moq;
using Shouldly;
using System.Text;

namespace Architecture.Shell.Correlation.Test.Messaging;

public class RabbitMqCorrelationProtocolHandlerTests
{
    private readonly RabbitMqCorrelationProtocolHandler _handler;

    public RabbitMqCorrelationProtocolHandlerTests()
    {
        _handler = new RabbitMqCorrelationProtocolHandler();
    }

    [Fact]
    public void MetadataType_ShouldReturnIBasicPropertiesType()
    {
        // Given & When & Then
        _handler.MetadataType.ShouldBe(typeof(IBasicProperties));
    }

    [Fact]
    public void Inject_WithBasicProperties_ShouldAddCorrelationHeader()
    {
        // Given
        var headers = new Dictionary<string, object>();
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        mockProperties.SetupSet(x => x.Headers = It.IsAny<IDictionary<string, object>>());
        
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(mockProperties.Object, headerName, correlationId);

        // Then
        headers.ShouldNotBeNull();
        headers.ShouldContainKey(headerName);
        headers[headerName].ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithExistingHeaders_ShouldAddCorrelationHeader()
    {
        // Given
        var headers = new Dictionary<string, object> { { "existing-header", "existing-value" } };
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(mockProperties.Object, headerName, correlationId);

        // Then
        headers.ShouldContainKey("existing-header");
        headers.ShouldContainKey(headerName);
        headers[headerName].ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithExistingCorrelationHeader_ShouldReplaceHeader()
    {
        // Given
        var headers = new Dictionary<string, object> { { "x-correlation-id", "old-value" } };
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        
        var newCorrelationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(mockProperties.Object, headerName, newCorrelationId);

        // Then
        headers[headerName].ShouldBe(newCorrelationId);
    }

    [Fact]
    public void Inject_WithNonBasicPropertiesObject_ShouldNotThrow()
    {
        // Given
        var nonProperties = new object();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When & Then
        Should.NotThrow(() => _handler.Inject(nonProperties, headerName, correlationId));
    }

    [Fact]
    public void Extract_WithStringCorrelationHeader_ShouldReturnCorrelationId()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var headers = new Dictionary<string, object> { { "x-correlation-id", correlationId } };
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(mockProperties.Object, headerName);

        // Then
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void Extract_WithByteArrayCorrelationHeader_ShouldReturnCorrelationId()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var correlationBytes = Encoding.UTF8.GetBytes(correlationId);
        var headers = new Dictionary<string, object> { { "x-correlation-id", correlationBytes } };
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(mockProperties.Object, headerName);

        // Then
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void Extract_WithObjectCorrelationHeader_ShouldReturnStringRepresentation()
    {
        // Given
        var correlationId = 12345;
        var headers = new Dictionary<string, object> { { "x-correlation-id", correlationId } };
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(mockProperties.Object, headerName);

        // Then
        result.ShouldBe("12345");
    }

    [Fact]
    public void Extract_WithMissingCorrelationHeader_ShouldReturnNull()
    {
        // Given
        var headers = new Dictionary<string, object> { { "other-header", "other-value" } };
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(mockProperties.Object, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithNullHeaders_ShouldReturnNull()
    {
        // Given
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns((IDictionary<string, object>?)null);
        
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(mockProperties.Object, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithNonBasicPropertiesObject_ShouldReturnNull()
    {
        // Given
        var nonProperties = new object();
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(nonProperties, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void RoundTrip_InjectAndExtract_ShouldPreserveCorrelationId()
    {
        // Given
        var headers = new Dictionary<string, object>();
        var mockProperties = new Mock<IBasicProperties>();
        mockProperties.SetupGet(x => x.Headers).Returns(headers);
        mockProperties.SetupSet(x => x.Headers = It.IsAny<IDictionary<string, object>>());
        
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(mockProperties.Object, headerName, correlationId);
        var extractedId = _handler.Extract(mockProperties.Object, headerName);

        // Then
        extractedId.ShouldBe(correlationId);
    }
}