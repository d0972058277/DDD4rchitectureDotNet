using Architecture;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Integration.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        SystemDateTime.InitUtcNow(() => DateTime.UtcNow);
    }
}
