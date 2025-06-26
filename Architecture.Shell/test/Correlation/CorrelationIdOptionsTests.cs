using Architecture.Shell.Correlation;
using Shouldly;

namespace Architecture.Shell.Test.Correlation;

public class CorrelationIdOptionsTests
{
    [Fact]
    public void DefaultHeader_ShouldHaveExpectedValue()
    {
        // Then
        CorrelationIdOptions.DefaultHeader.ShouldBe("X-Correlation-ID");
    }

    [Fact]
    public void LoggerScopeKey_ShouldHaveExpectedValue()
    {
        // Then
        CorrelationIdOptions.LoggerScopeKey.ShouldBe("CorrelationId");
    }

    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // When
        var options = new CorrelationIdOptions();

        // Then
        options.RequestHeader.ShouldBe(CorrelationIdOptions.DefaultHeader);
        options.ResponseHeader.ShouldBe(CorrelationIdOptions.DefaultHeader);
        options.IgnoreRequestHeader.ShouldBeFalse();
        options.EnforceHeader.ShouldBeFalse();
        options.AddToLoggingScope.ShouldBeFalse();
        options.LoggingScopeKey.ShouldBe(CorrelationIdOptions.LoggerScopeKey);
        options.IncludeInResponse.ShouldBeTrue();
        options.UpdateTraceIdentifier.ShouldBeFalse();
        options.CorrelationIdGenerator.ShouldBeNull();
    }

    [Fact]
    public void RequestHeader_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();
        var customHeader = "X-Custom-Correlation";

        // When
        options.RequestHeader = customHeader;

        // Then
        options.RequestHeader.ShouldBe(customHeader);
    }

    [Fact]
    public void ResponseHeader_WhenNotSet_ShouldReturnRequestHeader()
    {
        // Given
        var options = new CorrelationIdOptions();
        var customHeader = "X-Custom-Correlation";
        options.RequestHeader = customHeader;

        // When & Assert
        options.ResponseHeader.ShouldBe(customHeader);
    }

    [Fact]
    public void ResponseHeader_WhenSet_ShouldReturnSetValue()
    {
        // Given
        var options = new CorrelationIdOptions();
        var requestHeader = "X-Request-Correlation";
        var responseHeader = "X-Response-Correlation";
        options.RequestHeader = requestHeader;
        options.ResponseHeader = responseHeader;

        // When & Assert
        options.ResponseHeader.ShouldBe(responseHeader);
        options.RequestHeader.ShouldBe(requestHeader);
    }

    [Fact]
    public void IgnoreRequestHeader_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();

        // When
        options.IgnoreRequestHeader = true;

        // Then
        options.IgnoreRequestHeader.ShouldBeTrue();
    }

    [Fact]
    public void EnforceHeader_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();

        // When
        options.EnforceHeader = true;

        // Then
        options.EnforceHeader.ShouldBeTrue();
    }

    [Fact]
    public void AddToLoggingScope_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();

        // When
        options.AddToLoggingScope = true;

        // Then
        options.AddToLoggingScope.ShouldBeTrue();
    }

    [Fact]
    public void LoggingScopeKey_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();
        var customKey = "CustomCorrelationKey";

        // When
        options.LoggingScopeKey = customKey;

        // Then
        options.LoggingScopeKey.ShouldBe(customKey);
    }

    [Fact]
    public void IncludeInResponse_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();

        // When
        options.IncludeInResponse = false;

        // Then
        options.IncludeInResponse.ShouldBeFalse();
    }

    [Fact]
    public void UpdateTraceIdentifier_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();

        // When
        options.UpdateTraceIdentifier = true;

        // Then
        options.UpdateTraceIdentifier.ShouldBeTrue();
    }

    [Fact]
    public void CorrelationIdGenerator_CanBeSet()
    {
        // Given
        var options = new CorrelationIdOptions();
        Func<string> generator = () => "custom-id";

        // When
        options.CorrelationIdGenerator = generator;

        // Then
        options.CorrelationIdGenerator.ShouldBe(generator);
        options.CorrelationIdGenerator().ShouldBe("custom-id");
    }
}