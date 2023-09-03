using Architecture;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Unit.Tests;

public class Startup
{
    public static void ConfigureServices(IServiceCollection _)
    {
        var now = DateTime.UtcNow;
        SystemDateTime.InitUtcNow(() => now);
    }
}