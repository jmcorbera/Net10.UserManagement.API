using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Net10.UserManagement.Api.Bootstrapper;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = "User Management API";
                document.Info.Version = "v1";
                document.Info.Description = "API for user management with Clean Architecture";
                return Task.CompletedTask;
            });
        });
        services.AddEndpointsApiExplorer();
        services.AddAuthorization();
        services.AddBasicHealthChecks();
            
        return services;
    }

    private static IHealthChecksBuilder AddBasicHealthChecks(this IServiceCollection services)
    {
        return services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddCheck("self-ready", () => HealthCheckResult.Healthy(), tags: ["ready"]);
    }
}