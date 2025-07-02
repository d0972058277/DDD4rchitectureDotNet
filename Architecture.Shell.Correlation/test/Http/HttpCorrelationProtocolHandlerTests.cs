using Architecture.Shell.Correlation.Http;
using Shouldly;
using System.Net.Http.Headers;

namespace Architecture.Shell.Correlation.Test.Http;

public class HttpCorrelationProtocolHandlerTests
{
    private readonly HttpCorrelationProtocolHandler _handler;

    public HttpCorrelationProtocolHandlerTests()
    {
        _handler = new HttpCorrelationProtocolHandler();
    }

    [Fact]
    public void MetadataType_ShouldReturnHttpHeadersType()
    {
        // Given & When & Then
        _handler.MetadataType.ShouldBe(typeof(HttpHeaders));
    }

    [Fact]
    public void Inject_WithHttpRequestHeaders_ShouldAddCorrelationHeader()
    {
        // Given
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "X-Correlation-ID";

        // When
        _handler.Inject(request.Headers, headerName, correlationId);

        // Then
        request.Headers.GetValues(headerName).Single().ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithExistingCorrelationHeader_ShouldReplaceHeader()
    {
        // Given
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var oldCorrelationId = Guid.NewGuid().ToString();
        var newCorrelationId = Guid.NewGuid().ToString();
        var headerName = "X-Correlation-ID";
        
        request.Headers.Add(headerName, oldCorrelationId);

        // When
        _handler.Inject(request.Headers, headerName, newCorrelationId);

        // Then
        request.Headers.GetValues(headerName).Single().ShouldBe(newCorrelationId);
    }

    [Fact]
    public void Inject_WithNonHttpHeaders_ShouldNotThrow()
    {
        // Given
        var nonHttpHeaders = new object();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "X-Correlation-ID";

        // When & Then
        Should.NotThrow(() => _handler.Inject(nonHttpHeaders, headerName, correlationId));
    }

    [Fact]
    public void Extract_WithExistingCorrelationHeader_ShouldReturnCorrelationId()
    {
        // Given
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "X-Correlation-ID";
        
        request.Headers.Add(headerName, correlationId);

        // When
        var result = _handler.Extract(request.Headers, headerName);

        // Then
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void Extract_WithMissingCorrelationHeader_ShouldReturnNull()
    {
        // Given
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var headerName = "X-Correlation-ID";

        // When
        var result = _handler.Extract(request.Headers, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithMultipleValues_ShouldReturnFirstValue()
    {
        // Given
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var firstCorrelationId = Guid.NewGuid().ToString();
        var secondCorrelationId = Guid.NewGuid().ToString();
        var headerName = "X-Custom-Header"; // Use custom header to allow multiple values
        
        request.Headers.Add(headerName, new[] { firstCorrelationId, secondCorrelationId });

        // When
        var result = _handler.Extract(request.Headers, headerName);

        // Then
        result.ShouldBe(firstCorrelationId);
    }

    [Fact]
    public void Extract_WithNonHttpHeaders_ShouldReturnNull()
    {
        // Given
        var nonHttpHeaders = new object();
        var headerName = "X-Correlation-ID";

        // When
        var result = _handler.Extract(nonHttpHeaders, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithHttpResponseHeaders_ShouldExtractCorrelationId()
    {
        // Given
        var response = new HttpResponseMessage();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "X-Correlation-ID";
        
        response.Headers.Add(headerName, correlationId);

        // When
        var result = _handler.Extract(response.Headers, headerName);

        // Then
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithHttpResponseHeaders_ShouldAddCorrelationHeader()
    {
        // Given
        var response = new HttpResponseMessage();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "X-Correlation-ID";

        // When
        _handler.Inject(response.Headers, headerName, correlationId);

        // Then
        response.Headers.GetValues(headerName).Single().ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithEmptyCorrelationId_ShouldAddEmptyHeader()
    {
        // Given
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var correlationId = "";
        var headerName = "X-Correlation-ID";

        // When
        _handler.Inject(request.Headers, headerName, correlationId);

        // Then
        request.Headers.GetValues(headerName).Single().ShouldBe(correlationId);
    }
}