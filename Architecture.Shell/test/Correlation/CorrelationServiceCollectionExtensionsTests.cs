using Architecture.Shell.Correlation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace Architecture.Shell.Test.Correlation;

public class CorrelationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCorrelation_WithoutOptions_ShouldRegisterRequiredServices()
    {
        // Given
        var services = new ServiceCollection();

        // When
        services.AddCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        
        serviceProvider.GetService<ICorrelationContextAccessor>().ShouldNotBeNull();
        serviceProvider.GetService<ICorrelationContextFactory>().ShouldNotBeNull();
        serviceProvider.GetService<ICorrelationService>().ShouldNotBeNull();
        serviceProvider.GetService<IOptions<CorrelationIdOptions>>().ShouldNotBeNull();
    }

    [Fact]
    public void AddCorrelation_WithOptions_ShouldRegisterServicesAndApplyOptions()
    {
        // Given
        var services = new ServiceCollection();
        const string customHeader = "X-Custom-Header";

        // When
        services.AddCorrelation(options =>
        {
            options.RequestHeader = customHeader;
            options.EnforceHeader = true;
        });

        // Then
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<CorrelationIdOptions>>().Value;
        
        options.RequestHeader.ShouldBe(customHeader);
        options.EnforceHeader.ShouldBeTrue();
    }

    [Fact]
    public void AddCorrelation_CalledMultipleTimes_ShouldNotRegisterDuplicateServices()
    {
        // Given
        var services = new ServiceCollection();

        // When
        services.AddCorrelation();
        services.AddCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        var correlationServices = serviceProvider.GetServices<ICorrelationService>();
        correlationServices.Count().ShouldBe(1);
    }

    [Fact]
    public void AddCorrelation_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Given
        ServiceCollection services = null;

        // When & Assert
        Should.Throw<ArgumentNullException>(() => services.AddCorrelation())
            .ParamName.ShouldBe("services");
    }

    [Fact]
    public void AddCorrelation_WithNullConfigureOptions_ShouldThrowArgumentNullException()
    {
        // Given
        var services = new ServiceCollection();

        // When & Assert
        Should.Throw<ArgumentNullException>(() => services.AddCorrelation(null))
            .ParamName.ShouldBe("configureOptions");
    }

    [Fact]
    public void AddCorrelation_ShouldRegisterCorrectServiceLifetimes()
    {
        // Given
        var services = new ServiceCollection();

        // When
        services.AddCorrelation();

        // Then
        var accessorDescriptor = services.Single(x => x.ServiceType == typeof(ICorrelationContextAccessor));
        accessorDescriptor.Lifetime.ShouldBe(ServiceLifetime.Singleton);

        var factoryDescriptor = services.Single(x => x.ServiceType == typeof(ICorrelationContextFactory));
        factoryDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);

        var serviceDescriptor = services.Single(x => x.ServiceType == typeof(ICorrelationService));
        serviceDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }
}