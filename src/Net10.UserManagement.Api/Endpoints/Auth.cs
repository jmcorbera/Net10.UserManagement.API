using MediatR;
using Net10.UserManagement.Application.Auth.Commands.RegisterUser;

namespace Net10.UserManagement.Api.Endpoints;

public static class Auth
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("api/v1/auth");
        authGroup.MapPost("/register", Register)
            .WithName("RegisterUser")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user with Pending status and sends OTP via email");

    }
    
    private static async Task<IResult> Register(RegisterUserCommand command, IMediator mediator, CancellationToken cancellationToken)
    {
        var createdUser = await mediator.Send(command, cancellationToken);
        if (createdUser == null)
            return Results.BadRequest();

        return Results.Created($"/api/v1/users/{createdUser.Id}", createdUser);
    }
}
