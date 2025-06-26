using Architecture.Shell.Correlation;
using Microsoft.AspNetCore.Builder;
using Moq;
using Shouldly;

namespace Architecture.Shell.Test.Correlation;

public class CorrelationApplicationBuilderExtensionsTests
{
    [Fact]
    public void UseCorrelation_WithValidApplicationBuilder_ShouldNotThrow()
    {
        // Given
        var mockAppBuilder = new Mock<IApplicationBuilder>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockAppBuilder.Setup(x => x.ApplicationServices).Returns(mockServiceProvider.Object);

        // When & Then
        Should.NotThrow(() => mockAppBuilder.Object.UseCorrelation());
    }

    [Fact]
    public void UseCorrelation_WithNullApplicationBuilder_ShouldThrowArgumentNullException()
    {
        // Given
        IApplicationBuilder app = null;

        // When & Then
        Should.Throw<ArgumentNullException>(() => app.UseCorrelation())
            .ParamName.ShouldBe("app");
    }
}