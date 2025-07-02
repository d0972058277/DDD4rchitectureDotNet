using Architecture.Shell.Correlation;
using Architecture.Shell.Correlation.Http;
using Moq;
using Shouldly;

namespace Architecture.Shell.Correlation.Test.Http;

public class HttpCorrelationHandlerTests : IDisposable
{
    private readonly Mock<ICorrelationPropagator> _mockPropagator;
    private readonly HttpCorrelationHandler _handler;
    private readonly TestHttpMessageHandler _innerHandler;
    private readonly HttpClient _httpClient;

    public HttpCorrelationHandlerTests()
    {
        _mockPropagator = new Mock<ICorrelationPropagator>();
        _innerHandler = new TestHttpMessageHandler();
        _handler = new HttpCorrelationHandler(_mockPropagator.Object)
        {
            InnerHandler = _innerHandler
        };
        _httpClient = new HttpClient(_handler);
    }

    [Fact]
    public async Task SendAsync_ShouldInjectCorrelationIdIntoRequest()
    {
        // Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // When
        await _httpClient.SendAsync(request);

        // Then
        _mockPropagator.Verify(x => x.Inject(request.Headers), Times.Once);
    }

    [Fact]
    public async Task SendAsync_ShouldCallInnerHandler()
    {
        // Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // When
        var response = await _httpClient.SendAsync(request);

        // Then
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        _innerHandler.RequestReceived.ShouldBe(request);
    }

    [Fact]
    public async Task SendAsync_WhenPropagatorThrows_ShouldStillCallInnerHandler()
    {
        // Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        
        _mockPropagator.Setup(x => x.Inject(It.IsAny<object>()))
                      .Throws(new InvalidOperationException("Test exception"));

        // When & Then
        await Should.ThrowAsync<InvalidOperationException>(_httpClient.SendAsync(request));
    }

    [Fact]
    public async Task SendAsync_WithMultipleRequests_ShouldInjectEachRequest()
    {
        // Given
        var request1 = new HttpRequestMessage(HttpMethod.Get, "https://example.com/1");
        var request2 = new HttpRequestMessage(HttpMethod.Post, "https://example.com/2");

        // When
        await _httpClient.SendAsync(request1);
        await _httpClient.SendAsync(request2);

        // Then
        _mockPropagator.Verify(x => x.Inject(It.IsAny<object>()), Times.Exactly(2));
    }

    [Fact]
    public async Task SendAsync_WithDifferentHttpMethods_ShouldInjectForAll()
    {
        // Given
        var methods = new[] { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete, HttpMethod.Patch };

        // When
        foreach (var method in methods)
        {
            var request = new HttpRequestMessage(method, "https://example.com");
            await _httpClient.SendAsync(request);
        }

        // Then
        _mockPropagator.Verify(x => x.Inject(It.IsAny<object>()), Times.Exactly(methods.Length));
    }

    [Fact]
    public void Constructor_WithNullPropagator_ShouldThrowArgumentNullException()
    {
        // Given & When & Then
        Should.Throw<ArgumentNullException>(() => new HttpCorrelationHandler(null!))
            .ParamName.ShouldBe("correlationPropagator");
    }

    [Fact]
    public async Task SendAsync_WithCancellationRequested_ShouldStillInject()
    {
        // Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // When & Then
        await Should.ThrowAsync<OperationCanceledException>(
            _httpClient.SendAsync(request, cancellationTokenSource.Token));
            
        _mockPropagator.Verify(x => x.Inject(request.Headers), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithRequestContent_ShouldInjectRegardlessOfContent()
    {
        // Given
        var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("test content")
        };

        // When
        await _httpClient.SendAsync(request);

        // Then
        _mockPropagator.Verify(x => x.Inject(request.Headers), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithCustomHeaders_ShouldInjectCorrelationHeader()
    {
        // Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        request.Headers.Add("Custom-Header", "custom-value");

        // When
        await _httpClient.SendAsync(request);

        // Then
        _mockPropagator.Verify(x => x.Inject(request.Headers), Times.Once);
        request.Headers.GetValues("Custom-Header").Single().ShouldBe("custom-value");
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _handler?.Dispose();
        _innerHandler?.Dispose();
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? RequestReceived { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            RequestReceived = request;
            
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("Test response")
            });
        }
    }
}