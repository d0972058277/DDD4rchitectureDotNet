using Architecture.Application.CQRS;

namespace Architecture.Tests.Application.CQRS
{
    public partial class EventMediatorTests
    {
        public class SomethingCommand : ICommand { }
        public class SomethingBooleanCommand : ICommand<bool> { }
    }
}