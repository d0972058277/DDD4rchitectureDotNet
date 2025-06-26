using Architecture.Shell.Correlation.Messaging;
using Confluent.Kafka;
using Shouldly;
using System.Text;

namespace Architecture.Shell.Test.Correlation.Messaging;

public class KafkaCorrelationProtocolHandlerTests
{
    private readonly KafkaCorrelationProtocolHandler _handler;

    public KafkaCorrelationProtocolHandlerTests()
    {
        _handler = new KafkaCorrelationProtocolHandler();
    }

    [Fact]
    public void MetadataType_ShouldReturnHeadersType()
    {
        // Given & When & Then
        _handler.MetadataType.ShouldBe(typeof(Headers));
    }

    [Fact]
    public void Inject_WithHeaders_ShouldAddCorrelationHeader()
    {
        // Given
        var headers = new Headers();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(headers, headerName, correlationId);

        // Then
        headers.TryGetLastBytes(headerName, out var correlationBytes).ShouldBeTrue();
        Encoding.UTF8.GetString(correlationBytes).ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithExistingHeaders_ShouldAddCorrelationHeader()
    {
        // Given
        var headers = new Headers { { "existing-header", Encoding.UTF8.GetBytes("existing-value") } };
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(headers, headerName, correlationId);

        // Then
        headers.TryGetLastBytes("existing-header", out _).ShouldBeTrue();
        headers.TryGetLastBytes(headerName, out var correlationBytes).ShouldBeTrue();
        Encoding.UTF8.GetString(correlationBytes).ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithExistingCorrelationHeader_ShouldReplaceHeader()
    {
        // Given
        var headers = new Headers { { "x-correlation-id", Encoding.UTF8.GetBytes("old-value") } };
        var newCorrelationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(headers, headerName, newCorrelationId);

        // Then
        headers.TryGetLastBytes(headerName, out var correlationBytes).ShouldBeTrue();
        Encoding.UTF8.GetString(correlationBytes).ShouldBe(newCorrelationId);
    }

    [Fact]
    public void Inject_WithNonHeadersObject_ShouldNotThrow()
    {
        // Given
        var nonHeaders = new object();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When & Then
        Should.NotThrow(() => _handler.Inject(nonHeaders, headerName, correlationId));
    }

    [Fact]
    public void Extract_WithCorrelationHeader_ShouldReturnCorrelationId()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var correlationBytes = Encoding.UTF8.GetBytes(correlationId);
        var headers = new Headers { { "x-correlation-id", correlationBytes } };
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(headers, headerName);

        // Then
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void Extract_WithMissingCorrelationHeader_ShouldReturnNull()
    {
        // Given
        var headers = new Headers { { "other-header", Encoding.UTF8.GetBytes("other-value") } };
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(headers, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithNonHeadersObject_ShouldReturnNull()
    {
        // Given
        var nonHeaders = new object();
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(nonHeaders, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithEmptyHeaders_ShouldReturnNull()
    {
        // Given
        var headers = new Headers();
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(headers, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void RoundTrip_InjectAndExtract_ShouldPreserveCorrelationId()
    {
        // Given
        var headers = new Headers();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(headers, headerName, correlationId);
        var extractedId = _handler.Extract(headers, headerName);

        // Then
        extractedId.ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithUnicodeCorrelationId_ShouldHandleCorrectly()
    {
        // Given
        var headers = new Headers();
        var correlationId = "相關性識別碼-123-αβγ";
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(headers, headerName, correlationId);
        var extractedId = _handler.Extract(headers, headerName);

        // Then
        extractedId.ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithEmptyCorrelationId_ShouldHandleCorrectly()
    {
        // Given
        var headers = new Headers();
        var correlationId = "";
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(headers, headerName, correlationId);
        var extractedId = _handler.Extract(headers, headerName);

        // Then
        extractedId.ShouldBe(correlationId);
    }
}

