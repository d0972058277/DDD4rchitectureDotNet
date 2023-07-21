using Architecture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.Infrastructure;
using Project.Infrastructure.DbCommandInterceptors;
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
