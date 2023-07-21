using Architecture;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Unit.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var now = DateTime.UtcNow;
        SystemDateTime.InitUtcNow(() => now);
    }
}