using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Net10.UserManagement.Application.Common.Behaviors;

namespace Net10.UserManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
               .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
               .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)));
        
        services.AddAutoMapper(cfg => {}, Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}