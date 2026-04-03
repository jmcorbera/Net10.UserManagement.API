using Net10.UserManagement.Api.Bootstrapper;

namespace Net10.UserManagement.Api;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigServiceCollection(builder.Configuration);

            var app = builder.Build();

            app.ConfigureMiddleware();

            app.Run();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application failed with exception: {ex.Message}");
            throw;
        }
    }
}
