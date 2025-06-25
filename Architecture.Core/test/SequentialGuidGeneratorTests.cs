using Shouldly;

namespace Architecture.Core.Test;

public class SequentialGuidGeneratorTests
{
    [Fact]
    public void NewId_應該產生新的GUID()
    {
        // Given
        var sequentialGuidGenerator = new SequentialGuidGenerator();

        // When
        var id1 = sequentialGuidGenerator.NewId();
        var id2 = sequentialGuidGenerator.NewId();

        // Then
        id1.ShouldNotBe(Guid.Empty);
        id2.ShouldNotBe(Guid.Empty);
        id1.ShouldNotBe(id2);
    }
}