using Microsoft.Extensions.DependencyInjection;

namespace Architecture.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var now = DateTime.UtcNow;
        SystemDateTime.InitUtcNow(() => now);
    }
}
