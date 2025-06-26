#if !NETSTANDARD2_0
using Architecture.Shell.Correlation;
using Architecture.Shell.Correlation.Grpc;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Moq;
using Shouldly;

namespace Architecture.Shell.Test.Correlation.Grpc;

public class GrpcCorrelationClientInterceptorTests
{
    private readonly Mock<ICorrelationPropagator> _mockPropagator;
    private readonly GrpcCorrelationClientInterceptor _interceptor;

    public GrpcCorrelationClientInterceptorTests()
    {
        _mockPropagator = new Mock<ICorrelationPropagator>();
        _interceptor = new GrpcCorrelationClientInterceptor(_mockPropagator.Object);
    }

    [Fact]
    public void Constructor_WithNullPropagator_ShouldThrowArgumentNullException()
    {
        // Given & When & Then
        Should.Throw<ArgumentNullException>(() => new GrpcCorrelationClientInterceptor(null!))
            .ParamName.ShouldBe("correlationPropagator");
    }

    [Fact]
    public void Interceptor_ShouldBeInstanceOfInterceptor()
    {
        // Given & When & Then
        _interceptor.ShouldBeOfType<GrpcCorrelationClientInterceptor>();
        _interceptor.ShouldBeAssignableTo<Interceptor>();
    }
}

public class GrpcCorrelationServerInterceptorTests
{
    private readonly Mock<ICorrelationPropagator> _mockPropagator;
    private readonly Mock<ICorrelationContextFactory> _mockFactory;
    private readonly GrpcCorrelationServerInterceptor _interceptor;

    public GrpcCorrelationServerInterceptorTests()
    {
        _mockPropagator = new Mock<ICorrelationPropagator>();
        _mockFactory = new Mock<ICorrelationContextFactory>();
        _interceptor = new GrpcCorrelationServerInterceptor(_mockPropagator.Object, _mockFactory.Object);
    }

    [Fact]
    public void Constructor_WithNullPropagator_ShouldThrowArgumentNullException()
    {
        // Given & When & Then
        Should.Throw<ArgumentNullException>(() => new GrpcCorrelationServerInterceptor(null!, _mockFactory.Object))
            .ParamName.ShouldBe("correlationPropagator");
    }

    [Fact]
    public void Constructor_WithNullFactory_ShouldThrowArgumentNullException()
    {
        // Given & When & Then
        Should.Throw<ArgumentNullException>(() => new GrpcCorrelationServerInterceptor(_mockPropagator.Object, null!))
            .ParamName.ShouldBe("correlationContextFactory");
    }

    [Fact]
    public void Interceptor_ShouldBeInstanceOfInterceptor()
    {
        // Given & When & Then
        _interceptor.ShouldBeOfType<GrpcCorrelationServerInterceptor>();
        _interceptor.ShouldBeAssignableTo<Interceptor>();
    }

}
#endif