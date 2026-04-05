using MediatR;
using Net10.UserManagement.Application.Users.Commands.CreateUser;
using Net10.UserManagement.Application.Users.Commands.DeleteUser;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;
using Net10.UserManagement.Application.Users.Queries.GetUsers;
using Net10.UserManagement.Application.Users.Queries.GetUserById;

namespace Net10.UserManagement.Api.Endpoints;

public static class Users
{
    
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {     
        var usersGroup = app.MapGroup("api/v1/users")
            .WithTags("Users");

        usersGroup.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get all users")
            .WithDescription("Retrieves a list of all users");

        usersGroup.MapGet("/{id}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieves a user by their ID");

        usersGroup.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("Creates a new user");

        usersGroup.MapPut("/{id}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update an existing user")
            .WithDescription("Updates an existing user");

        usersGroup.MapDelete("/{id}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete a user")
            .WithDescription("Deletes a user by their ID");
     
    }

    private static async Task<IResult> GetUsers(IMediator mediator, CancellationToken cancellationToken)
    {
        var users = await mediator.Send(new GetUsersQuery(), cancellationToken);
        if (users == null || !users.Any())
            return Results.NotFound();

        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserById(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        if (user == null)
            return Results.NotFound();

        return Results.Ok(user);
    }

    private static async Task<IResult> CreateUser(CreateUserCommand user, IMediator mediator, CancellationToken cancellationToken)
    {
        var createdUser = await mediator.Send(user, cancellationToken);
        if (createdUser == null)
            return Results.BadRequest();

        return Results.Created($"/api/v1/users/{createdUser.Id}", createdUser);
    }

    private static async Task<IResult> UpdateUser(Guid id, UpdateUserCommand user, IMediator mediator, CancellationToken cancellationToken)
    {
        user = user with { Id = id };
        var updatedUser = await mediator.Send(user, cancellationToken);
        if (updatedUser == null)
            return Results.NotFound();

        return Results.Ok(updatedUser);
    }

    private static async Task<IResult> DeleteUser(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return Results.NoContent();
    }

}