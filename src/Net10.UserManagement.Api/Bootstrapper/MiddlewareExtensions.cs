using Net10.UserManagement.Api.Endpoints;
using Scalar.AspNetCore;

namespace Net10.UserManagement.Api.Bootstrapper;

public static class MiddlewareExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.DarkMode = true;
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapUsersEndpoints();
        app.MapHealthEndpoints();

        return app;
    }
}
