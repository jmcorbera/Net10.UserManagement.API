using Microsoft.Extensions.DependencyInjection;
using Net10.UserManagement.Application.Abstracts;
using Net10.UserManagement.Application.Users.Services;

namespace Net10.UserManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}