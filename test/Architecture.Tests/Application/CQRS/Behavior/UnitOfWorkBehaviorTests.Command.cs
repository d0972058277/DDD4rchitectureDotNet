using Architecture.Application.CQRS;

namespace Architecture.Tests.Application.CQRS.Behavior
{
    public partial class UnitOfWorkBehaviorTests
    {
        public class SomethingCommand : ICommand { }

        public class SomethingCommandHandler : ICommandHandler<SomethingCommand>
        {
            public Task Handle(SomethingCommand request, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}