using System.Reflection;
using Architecture;
using Architecture.Shell;
using Architecture.Shell.CQRS;
using Architecture.Shell.CQRS.Behavior;
using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Inbox;
using Architecture.Shell.EventBus.Outbox;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.Infrastructure.EventBusContext.Outbox;

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
        services.AddTransient<IEventPublisher, MasstransitEventPublisher>();
        services.AddTransient<IEventConsumer, EventConsumer>();
        services.AddScoped<IOutboxWorker, OutboxWorker>();
        services.AddSingleton<IInboxWorker, InboxWorker>();

        services.AddIntegrationEventHandlers();

        services.AddTransient<IFireAndForgetService, HangfireFireAndForgetService>();

        services.AddScoped<IUnitOfWork>(sp =>
        {
            var dbContext = sp.GetRequiredService<ProjectDbContext>();
            var fireAndForgetService = sp.GetRequiredService<IFireAndForgetService>();
            var logger = sp.GetRequiredService<ILogger<OutboxDecoratorUnitOfWork>>();

            var unitOfWork = new UnitOfWork(dbContext);
            var outboxDecoratorUnitOfWork = new OutboxDecoratorUnitOfWork(unitOfWork, fireAndForgetService, logger);
            return outboxDecoratorUnitOfWork;
        });

        services.AddHangfire(config =>
        {
            config.UseInMemoryStorage();
        }).AddHangfireServer(options =>
        {

        });

        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services)
    {
        var integrationEventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetLoadableTypes())
            .Where(t => typeof(IIntegrationEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass).ToList();

        foreach (var integrationEventType in integrationEventTypes)
        {
            var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(integrationEventType);
            var handlerImplementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && handlerType.IsAssignableFrom(type)).ToList();

            if (handlerImplementations.Count > 1)
                throw new InvalidOperationException($"一個 Domain 中，只可以有一個 {integrationEventType.Name} 的 IntegrationEventHandler 實作");

            if (handlerImplementations.Count == 1)
            {
                var handlerImplementation = handlerImplementations.First();
                services.AddScoped(handlerType, handlerImplementation);
            }
        }

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
