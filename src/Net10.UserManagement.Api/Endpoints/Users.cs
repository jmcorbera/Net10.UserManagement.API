using Net10.UserManagement.Application.Abstracts;
using Net10.UserManagement.Application.Users.Models;

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

    private static async Task<IResult> GetUsers(IUserService userService)
    {
        var users = await userService.GetAllAsync();
        if (users == null || !users.Any())
            return Results.NotFound();

        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserById(IUserService userService, Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null)
            return Results.NotFound();

        return Results.Ok(user);
    }

    private static async Task<IResult> CreateUser(IUserService userService, UserCreateCommand user)
    {
        var createdUser = await userService.CreateAsync(user);
        if (createdUser == null)
            return Results.BadRequest();

        return Results.Created($"/api/v1/users/{createdUser.Id}", createdUser);
    }

    private static async Task<IResult> UpdateUser(IUserService userService, Guid id, UserUpdateCommand user)
    {
        var updatedUser = await userService.UpdateAsync(id, user);
        if (updatedUser == null)
            return Results.NotFound();

        return Results.Ok(updatedUser);
    }

    private static async Task<IResult> DeleteUser(IUserService userService, Guid id)
    {
        await userService.DeleteAsync(id);
        return Results.NoContent();
    }

}