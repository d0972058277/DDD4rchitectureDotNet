using Architecture.Shell.Correlation;
using Shouldly;

namespace Architecture.Shell.Correlation.Test;

public class CorrelationContextAccessorTests
{
    [Fact]
    public void CorrelationContext_InitiallyNull()
    {
        // Given
        var accessor = new CorrelationContextAccessor();

        // When & Assert
        accessor.CorrelationContext.ShouldBeNull();
    }

    [Fact]
    public void CorrelationContext_CanSetAndGetValue()
    {
        // Given
        var accessor = new CorrelationContextAccessor();
        var context = new CorrelationContext(Guid.NewGuid().ToString(), "X-Correlation-ID");

        // When
        accessor.CorrelationContext = context;

        // Then
        accessor.CorrelationContext.ShouldBe(context);
    }

    [Fact]
    public void CorrelationContext_CanSetToNull()
    {
        // Given
        var accessor = new CorrelationContextAccessor();
        var context = new CorrelationContext(Guid.NewGuid().ToString(), "X-Correlation-ID");
        accessor.CorrelationContext = context;

        // When
        accessor.CorrelationContext = null;

        // Then
        accessor.CorrelationContext.ShouldBeNull();
    }

    [Fact]
    public void CorrelationContext_ShouldBeThreadSafe()
    {
        // Given
        var accessor = new CorrelationContextAccessor();
        var context1 = new CorrelationContext("id1", "header1");
        var context2 = new CorrelationContext("id2", "header2");
        var results = new CorrelationContext[2];

        // When
        var task1 = Task.Run(() =>
        {
            accessor.CorrelationContext = context1;
            Thread.Sleep(50);
            results[0] = accessor.CorrelationContext;
        });

        var task2 = Task.Run(() =>
        {
            accessor.CorrelationContext = context2;
            Thread.Sleep(50);
            results[1] = accessor.CorrelationContext;
        });

        Task.WaitAll(task1, task2);

        // Then
        results[0].ShouldBe(context1);
        results[1].ShouldBe(context2);
    }
}