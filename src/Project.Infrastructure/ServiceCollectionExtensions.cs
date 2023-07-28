using System.Reflection;
using Architecture.Application;
using Architecture.Application.CQRS;
using Architecture.Application.CQRS.Behavior;
using Architecture.Application.EventBus;
using Architecture.Application.EventBus.Outbox;
using Architecture.Application.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        services.AddAllTypes<IRepository>(ServiceLifetime.Transient);

        services.AddTransient<IEventMediator, EventMediator>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.Load("Project.Application"));
            cfg.RegisterServicesFromAssembly(Assembly.Load("Project.Infrastructure"));
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.AddDbContext<ProjectDbContext>(transationalDbContextOptionsAction);
        services.AddDbContext<ReadOnlyProjectDbContext>(readOnlyDbContextOptionsAction);

        services.AddTransient<IEventOutbox, EventOutbox>();

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
