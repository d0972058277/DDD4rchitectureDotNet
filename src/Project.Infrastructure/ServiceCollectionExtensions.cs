using System.Reflection;
using Architecture.Shell;
using Architecture.Shell.CQRS;
using Architecture.Shell.CQRS.Behavior;
using Architecture.Shell.EventBus.Inbox;
using Architecture.Shell.EventBus.Outbox;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProject
    (
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> transationalDbContextOptionsAction,
        Action<IServiceProvider, DbContextOptionsBuilder> readOnlyDbContextOptionsAction
    )
    {
        services.AddDefaultCorrelationId(options =>
        {
            options.AddToLoggingScope = true;
            options.RequestHeader = CorrelationIdOptions.DefaultHeader.ToLower();
        });

        services.AddAllTypes<IRepository>(ServiceLifetime.Transient);
        services.AddAllTypes<IApplicationService>(ServiceLifetime.Transient);

        services.AddTransient<IMediator, Mediator>();


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.Load("Project.Application"));
            cfg.RegisterServicesFromAssembly(Assembly.Load("Project.Infrastructure"));
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.AddDbContext<ProjectDbContext>(transationalDbContextOptionsAction);
        services.AddDbContext<ReadOnlyProjectDbContext>(readOnlyDbContextOptionsAction);

        services.AddTransient<IOutbox, Outbox>();
        services.AddTransient<IInbox, Inbox>();
        services.AddTransient<IEventPublisher, Masstransit.EventPublisher>();
        services.AddTransient<IEventConsumer, EventConsumer>();
        services.AddSingleton<IOutboxWorker, OutboxWorker>();
        services.AddSingleton<IInboxWorker, InboxWorker>();

        services.AddScoped<IUnitOfWork>(sp =>
        {
            var dbContext = sp.GetRequiredService<ProjectDbContext>();
            var outboxWorker = sp.GetRequiredService<IOutboxWorker>();
            var logger = sp.GetRequiredService<ILogger<UnitOfWorkOutboxDecorator>>();

            IUnitOfWork unitOfWork = new UnitOfWork(dbContext);
            unitOfWork = new UnitOfWorkOutboxDecorator(unitOfWork, outboxWorker, logger);
            return unitOfWork;
        });

        return services;
    }

    private static IServiceCollection AddAllTypes<T>(this IServiceCollection services, ServiceLifetime lifetime)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T)) && !x.IsInterface && !x.IsAbstract)).ToList();
        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(type, type, lifetime));

            var interfaces = type.GetInterfaces().ToList();
            foreach (var @interface in interfaces)
            {
                services.Add(new ServiceDescriptor(@interface, sp => sp.GetRequiredService(type), lifetime));
            }
        }
        return services;
    }
}
