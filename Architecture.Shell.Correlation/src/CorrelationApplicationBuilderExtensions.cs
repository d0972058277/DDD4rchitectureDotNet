using System;
using Microsoft.AspNetCore.Builder;

namespace Architecture.Shell.Correlation;

public static class CorrelationApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        return app.UseMiddleware<CorrelationMiddleware>();
    }
}