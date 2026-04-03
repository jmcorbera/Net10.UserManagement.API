using Net10.UserManagement.Application.Abstracts;

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
    }

    private static async Task<IResult> GetUsers(IUserService userService)
    {
        var users = await userService.GetAllAsync();
        if (users == null || !users.Any())
            return Results.NotFound();

        return Results.Ok(users);
    }
}