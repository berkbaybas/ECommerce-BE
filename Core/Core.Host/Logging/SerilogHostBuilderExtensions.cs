using Core.Host.Hosting;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Core.Host.Logging;

public static class SerilogHostBuilderExtensions
{
    public static WebApplicationBuilder AddCoreSerilog(this WebApplicationBuilder builder, ServiceInfo serviceInfo)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithProperty("Service", serviceInfo.Name)
            .WriteTo.Console());

        return builder;
    }
}
