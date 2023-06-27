namespace Architecture.Tests
{
    public class GenericTypeExtensionsTests
    {
        [Fact]
        public void 一般物件Type使用GetGenericTypeName_應該取得類別的名稱()
        {
            // Given
            var type = new GenericTypeExtensionsTests().GetType();

            // When
            var genericTypeName = type.GetGenericTypeName();

            // Then
            genericTypeName.Should().Be(nameof(GenericTypeExtensionsTests));
        }

        [Fact]
        public void 一般物件使用GetGenericTypeName_應該取得類別的名稱()
        {
            // Given
            var obj = new GenericTypeExtensionsTests();

            // When
            var genericTypeName = obj.GetGenericTypeName();

            // Then
            genericTypeName.Should().Be(nameof(GenericTypeExtensionsTests));
        }

        [Fact]
        public void 範型物件Type使用GetGenericTypeName_應該取得範型類別的名稱()
        {
            // Given
            var type = new List<GenericTypeExtensionsTests> { }.GetType();

            // When
            var genericTypeName = type.GetGenericTypeName();

            // Then
            genericTypeName.Should().Be($"List<{nameof(GenericTypeExtensionsTests)}>");
        }

        [Fact]
        public void 範型物件使用GetGenericTypeName_應該取得範型類別的名稱()
        {
            // Given
            var objs = new List<GenericTypeExtensionsTests> { };

            // When
            var genericTypeName = objs.GetGenericTypeName();

            // Then
            genericTypeName.Should().Be($"List<{nameof(GenericTypeExtensionsTests)}>");
        }
    }
}