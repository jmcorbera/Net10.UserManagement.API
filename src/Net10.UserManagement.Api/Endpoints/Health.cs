namespace Net10.UserManagement.Api.Endpoints
{
    public static class Health
    {
        public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            }).ExcludeFromDescription();
            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            }).ExcludeFromDescription();
        }
    }
}