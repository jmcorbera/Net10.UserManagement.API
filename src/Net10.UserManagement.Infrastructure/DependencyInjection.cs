using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Net10.UserManagement.Application.Common.Abstractions;
using Net10.UserManagement.Domain.Repositories;
using Net10.UserManagement.Infrastructure.Options;
using Net10.UserManagement.Infrastructure.Repositories;
using Net10.UserManagement.Infrastructure.Services;

namespace Net10.UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OtpGeneratorOptions>(configuration.GetSection(OtpGeneratorOptions.SectionName));

        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserOtpRepository, UserOtpRepository>();
        services.AddSingleton<IOtpSettingsProvider, OtpSettingsProvider>();
        services.AddSingleton<IOtpGenerator, SecureOtpGenerator>();
        services.AddSingleton<IEmailSender, ConsoleEmailSender>();

        return services;
    }
}
