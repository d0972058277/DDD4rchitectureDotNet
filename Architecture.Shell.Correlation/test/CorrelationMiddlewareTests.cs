using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System.Net;

namespace Architecture.Shell.Correlation.Test;

public class CorrelationMiddlewareTests : IDisposable
{
    private readonly IHost _host;
    private readonly HttpClient _client;

    public CorrelationMiddlewareTests()
    {
        _host = CreateHost();
        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task InvokeAsync_WithExistingCorrelationId_ShouldUseExistingId()
    {
        // Given
        var existingCorrelationId = Guid.NewGuid().ToString();
        _client.DefaultRequestHeaders.Add("X-Correlation-ID", existingCorrelationId);

        // When
        var response = await _client.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.GetValues("X-Correlation-ID").Single().ShouldBe(existingCorrelationId);
    }

    [Fact]
    public async Task InvokeAsync_WithoutCorrelationId_ShouldGenerateNewId()
    {
        // When
        var response = await _client.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var correlationId = response.Headers.GetValues("X-Correlation-ID").Single();
        Guid.TryParse(correlationId, out _).ShouldBeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WithCustomHeader_ShouldUseCustomHeader()
    {
        // Given
        var hostWithCustomHeader = CreateHost(options => options.RequestHeader = "X-Custom-Correlation");
        hostWithCustomHeader.Start();
        var clientWithCustomHeader = hostWithCustomHeader.GetTestClient();
        var correlationId = Guid.NewGuid().ToString();
        clientWithCustomHeader.DefaultRequestHeaders.Add("X-Custom-Correlation", correlationId);

        // When
        var response = await clientWithCustomHeader.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.GetValues("X-Custom-Correlation").Single().ShouldBe(correlationId);

        // Cleanup
        hostWithCustomHeader.Dispose();
        clientWithCustomHeader.Dispose();
    }

    [Fact]
    public async Task InvokeAsync_WithEnforceHeaderAndMissingHeader_ShouldReturnBadRequest()
    {
        // Given
        var hostWithEnforcement = CreateHost(options => options.EnforceHeader = true);
        hostWithEnforcement.Start();
        var clientWithEnforcement = hostWithEnforcement.GetTestClient();

        // When
        var response = await clientWithEnforcement.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Header 'X-Correlation-ID' is required.");

        // Cleanup
        hostWithEnforcement.Dispose();
        clientWithEnforcement.Dispose();
    }

    [Fact]
    public async Task InvokeAsync_WithIgnoreRequestHeader_ShouldIgnoreExistingHeader()
    {
        // Given
        var hostWithIgnore = CreateHost(options => options.IgnoreRequestHeader = true);
        hostWithIgnore.Start();
        var clientWithIgnore = hostWithIgnore.GetTestClient();
        var existingCorrelationId = Guid.NewGuid().ToString();
        clientWithIgnore.DefaultRequestHeaders.Add("X-Correlation-ID", existingCorrelationId);

        // When
        var response = await clientWithIgnore.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseCorrelationId = response.Headers.GetValues("X-Correlation-ID").Single();
        responseCorrelationId.ShouldNotBe(existingCorrelationId);
        Guid.TryParse(responseCorrelationId, out _).ShouldBeTrue();

        // Cleanup
        hostWithIgnore.Dispose();
        clientWithIgnore.Dispose();
    }

    [Fact]
    public async Task InvokeAsync_WithCustomGenerator_ShouldUseCustomGenerator()
    {
        // Given
        const string customId = "custom-generated-id";
        var hostWithCustomGenerator = CreateHost(options => options.CorrelationIdGenerator = () => customId);
        hostWithCustomGenerator.Start();
        var clientWithCustomGenerator = hostWithCustomGenerator.GetTestClient();

        // When
        var response = await clientWithCustomGenerator.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.GetValues("X-Correlation-ID").Single().ShouldBe(customId);

        // Cleanup
        hostWithCustomGenerator.Dispose();
        clientWithCustomGenerator.Dispose();
    }

    [Fact]
    public async Task InvokeAsync_WithIncludeInResponseFalse_ShouldNotIncludeResponseHeader()
    {
        // Given
        var hostWithoutResponse = CreateHost(options => options.IncludeInResponse = false);
        hostWithoutResponse.Start();
        var clientWithoutResponse = hostWithoutResponse.GetTestClient();

        // When
        var response = await clientWithoutResponse.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Contains("X-Correlation-ID").ShouldBeFalse();

        // Cleanup
        hostWithoutResponse.Dispose();
        clientWithoutResponse.Dispose();
    }

    [Fact]
    public async Task InvokeAsync_WithDifferentResponseHeader_ShouldUseDifferentResponseHeader()
    {
        // Given
        var correlationId = Guid.NewGuid().ToString();
        var hostWithDifferentResponse = CreateHost(options =>
        {
            options.RequestHeader = "X-Request-Correlation";
            options.ResponseHeader = "X-Response-Correlation";
        });
        hostWithDifferentResponse.Start();
        var clientWithDifferentResponse = hostWithDifferentResponse.GetTestClient();
        clientWithDifferentResponse.DefaultRequestHeaders.Add("X-Request-Correlation", correlationId);

        // When
        var response = await clientWithDifferentResponse.GetAsync("/test");

        // Then
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.GetValues("X-Response-Correlation").Single().ShouldBe(correlationId);
        response.Headers.Contains("X-Request-Correlation").ShouldBeFalse();

        // Cleanup
        hostWithDifferentResponse.Dispose();
        clientWithDifferentResponse.Dispose();
    }

    private static IHost CreateHost(Action<CorrelationIdOptions>? configureOptions = null)
    {
        return new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        if (configureOptions != null)
                        {
                            services.AddCorrelation(configureOptions);
                        }
                        else
                        {
                            services.AddCorrelation();
                        }
                        services.AddLogging();
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseCorrelation();
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/test", async context =>
                            {
                                await context.Response.WriteAsync("OK");
                            });
                        });
                    });
            })
            .Build();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _host?.Dispose();
    }
}