using Architecture;
using Architecture.Shell.EventBus;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.Infrastructure;
using Project.Infrastructure.DbCommandInterceptors;
using Project.Infrastructure.EventBusContext.Inbox;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

namespace Project.Integration.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
    {
        var connectionString = hostBuilderContext.Configuration.GetValue<string>("MySqlConnectionString");

        SystemDateTime.InitUtcNow(() =>
        {
            var now = DateTime.UtcNow;
            return DateTime.Parse(now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        });

        services.AddProject((sp, b) =>
        {
            b.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            b.AddInterceptors(new List<IInterceptor>() { new RemoveLastOrderByInterceptor() });
        }, (sp, b) =>
        {
            b.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            b.AddInterceptors(new List<IInterceptor>() { new RemoveLastOrderByInterceptor() });
        });

        services.AddMassTransitTestHarness(x =>
        {
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(t => typeof(IIntegrationEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .ToList()
                .ForEach(integrationEventType =>
                {
                    x.AddConsumer(typeof(MasstransitGenericConsumer<>).MakeGenericType(integrationEventType));
                });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
                cfg.Publish<IIntegrationEvent>(c => c.Exclude = true);
            });
        });
    }

    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder
            .ConfigureHostConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json", true);
                builder.AddEnvironmentVariables();
            })
            .UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                loggerConfiguration.ReadFrom.Configuration(configuration)
                    .Enrich.WithExceptionDetails
                    (
                        new DestructuringOptionsBuilder().WithDefaultDestructurers().WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
                    )
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyName)
                    .Enrich.WithProperty("EnvironmentName", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!)
                    .Enrich.WithProperty("RuntimeId", Guid.NewGuid().ToString());
            });
    }
}
