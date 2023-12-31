using System.Reflection;
using Architecture.Shell;
using Architecture.Shell.CQRS;
using Architecture.Shell.CQRS.Behavior;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Shell.CQRS.Behavior
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

        [Fact]
        public async Task UnitOfWork沒有活躍的Transaction_當執行Command_應該開啟Transaction_並在Command完成後Commit_最後進行OutboxProcess()
        {
            // Given
            var transactionId = Guid.NewGuid();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.BeginTransactionAsync(It.IsAny<CancellationToken>())).Callback(() =>
            {
                unitOfWork.Setup(m => m.TransactionId).Returns(transactionId);
                unitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);
            });
            var logger = new Mock<ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>>();

            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Architecture.Tests")));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
            services.AddTransient(typeof(IUnitOfWork), sp => unitOfWork.Object);
            services.AddTransient(typeof(ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>), sp => logger.Object);
            using var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<MediatR.IMediator>();

            var command = new SomethingCommand();

            // When
            await mediator.Send(command);

            // Then
            unitOfWork.Verify(m => m.BeginTransactionAsync(default), Times.Once());
            unitOfWork.Verify(m => m.CommitAsync(default), Times.Once());
            logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.StartsWith("----- Begin transaction")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
            logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.StartsWith("----- Commit transaction")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        }

        [Fact]
        public async Task UnitOfWork有活躍的Transaction_當執行Command_應該續用Transaction()
        {
            // Given
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.TransactionId).Returns(Guid.Empty);
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);
            var logger = new Mock<ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>>();

            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Architecture.Tests")));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
            services.AddTransient(typeof(IUnitOfWork), sp => unitOfWork.Object);
            services.AddTransient(typeof(ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>), sp => logger.Object);
            using var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<MediatR.IMediator>();

            var command = new SomethingCommand();

            // When
            await mediator.Send(command);

            // Then
            unitOfWork.Verify(m => m.BeginTransactionAsync(default), Times.Never());
            logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.StartsWith("----- Continue transaction")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        }

        [Fact]
        public async Task 當拋出例外且CancellationToken不是IsCancellationRequested_應該LogError()
        {
            // Given
            var exception = new Exception();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.BeginTransactionAsync(It.IsAny<CancellationToken>())).ThrowsAsync(exception);
            var logger = new Mock<ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>>();

            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Architecture.Tests")));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
            services.AddTransient(typeof(IUnitOfWork), sp => unitOfWork.Object);
            services.AddTransient(typeof(ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>), sp => logger.Object);
            using var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<MediatR.IMediator>();

            var command = new SomethingCommand();

            // When
            var action = () => mediator.Send(command);

            // Then
            await action.Should().ThrowAsync<Exception>();
            logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.StartsWith("ERROR Handling transaction")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        }

        [Fact]
        public async Task 當拋出例外且CancellationToken是IsCancellationRequested_應該LogInformation()
        {
            // Given
            var exception = new Exception();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.BeginTransactionAsync(It.IsAny<CancellationToken>())).ThrowsAsync(exception);
            var logger = new Mock<ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>>();

            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Architecture.Tests")));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
            services.AddTransient(typeof(IUnitOfWork), sp => unitOfWork.Object);
            services.AddTransient(typeof(ILogger<UnitOfWorkBehavior<SomethingCommand, MediatR.Unit>>), sp => logger.Object);
            using var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<MediatR.IMediator>();

            var command = new SomethingCommand();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // When
            var action = () => mediator.Send(command, cancellationTokenSource.Token);

            // Then
            await action.Should().ThrowAsync<Exception>();
            logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.StartsWith("ERROR Handling transaction")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        }
    }
}