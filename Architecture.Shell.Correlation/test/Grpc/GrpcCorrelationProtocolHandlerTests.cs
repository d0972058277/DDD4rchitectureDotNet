#if !NETSTANDARD2_0
using Architecture.Shell.Correlation.Grpc;
using Grpc.Core;
using Shouldly;

namespace Architecture.Shell.Correlation.Test.Grpc;

public class GrpcCorrelationProtocolHandlerTests
{
    private readonly GrpcCorrelationProtocolHandler _handler;

    public GrpcCorrelationProtocolHandlerTests()
    {
        _handler = new GrpcCorrelationProtocolHandler();
    }

    [Fact]
    public void MetadataType_ShouldReturnMetadataType()
    {
        // Given & When & Then
        _handler.MetadataType.ShouldBe(typeof(Metadata));
    }

    [Fact]
    public void Inject_WithMetadata_ShouldAddCorrelationEntry()
    {
        // Given
        var metadata = new Metadata();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(metadata, headerName, correlationId);

        // Then
        var entry = metadata.FirstOrDefault(e => e.Key == headerName.ToLowerInvariant());
        entry.ShouldNotBeNull();
        entry.Value.ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithExistingCorrelationEntry_ShouldReplaceEntry()
    {
        // Given
        var metadata = new Metadata();
        var oldCorrelationId = Guid.NewGuid().ToString();
        var newCorrelationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";
        
        metadata.Add(headerName, oldCorrelationId);

        // When
        _handler.Inject(metadata, headerName, newCorrelationId);

        // Then
        var entries = metadata.Where(e => string.Equals(e.Key, headerName, StringComparison.OrdinalIgnoreCase)).ToList();
        entries.Count.ShouldBe(1);
        entries.Single().Value.ShouldBe(newCorrelationId);
    }

    [Fact]
    public void Inject_WithCaseInsensitiveHeaderName_ShouldHandleCorrectly()
    {
        // Given
        var metadata = new Metadata();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "X-CORRELATION-ID";

        // When
        _handler.Inject(metadata, headerName, correlationId);

        // Then
        var entry = metadata.FirstOrDefault(e => e.Key == headerName.ToLowerInvariant());
        entry.ShouldNotBeNull();
        entry.Value.ShouldBe(correlationId);
    }

    [Fact]
    public void Inject_WithNonMetadataObject_ShouldNotThrow()
    {
        // Given
        var nonMetadata = new object();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When & Then
        Should.NotThrow(() => _handler.Inject(nonMetadata, headerName, correlationId));
    }

    [Fact]
    public void Extract_WithExistingCorrelationEntry_ShouldReturnCorrelationId()
    {
        // Given
        var metadata = new Metadata();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";
        
        metadata.Add(headerName, correlationId);

        // When
        var result = _handler.Extract(metadata, headerName);

        // Then
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void Extract_WithMissingCorrelationEntry_ShouldReturnNull()
    {
        // Given
        var metadata = new Metadata();
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(metadata, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithCaseInsensitiveHeaderName_ShouldFindEntry()
    {
        // Given
        var metadata = new Metadata();
        var correlationId = Guid.NewGuid().ToString();
        var headerNameLower = "x-correlation-id";
        var headerNameUpper = "X-CORRELATION-ID";
        
        metadata.Add(headerNameLower, correlationId);

        // When
        var result = _handler.Extract(metadata, headerNameUpper);

        // Then
        result.ShouldBe(correlationId);
    }

    [Fact]
    public void Extract_WithMultipleEntriesWithSameName_ShouldReturnFirstValue()
    {
        // Given
        var metadata = new Metadata();
        var firstCorrelationId = Guid.NewGuid().ToString();
        var secondCorrelationId = Guid.NewGuid().ToString();
        var headerName = "x-custom-header";
        
        metadata.Add(headerName, firstCorrelationId);
        metadata.Add(headerName, secondCorrelationId);

        // When
        var result = _handler.Extract(metadata, headerName);

        // Then
        result.ShouldBe(firstCorrelationId);
    }

    [Fact]
    public void Extract_WithNonMetadataObject_ShouldReturnNull()
    {
        // Given
        var nonMetadata = new object();
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(nonMetadata, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Extract_WithEmptyMetadata_ShouldReturnNull()
    {
        // Given
        var metadata = new Metadata();
        var headerName = "x-correlation-id";

        // When
        var result = _handler.Extract(metadata, headerName);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Inject_WithEmptyCorrelationId_ShouldAddEmptyEntry()
    {
        // Given
        var metadata = new Metadata();
        var correlationId = "";
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(metadata, headerName, correlationId);

        // Then
        var entry = metadata.FirstOrDefault(e => e.Key == headerName);
        entry.ShouldNotBeNull();
        entry.Value.ShouldBe(correlationId);
    }

    [Fact]
    public void RoundTrip_InjectAndExtract_ShouldPreserveCorrelationId()
    {
        // Given
        var metadata = new Metadata();
        var correlationId = Guid.NewGuid().ToString();
        var headerName = "x-correlation-id";

        // When
        _handler.Inject(metadata, headerName, correlationId);
        var extractedId = _handler.Extract(metadata, headerName);

        // Then
        extractedId.ShouldBe(correlationId);
    }
}
#endif