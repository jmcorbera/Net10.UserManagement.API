namespace Net10.UserManagement.Application.Users.Commands.CreateUser;

public class CreateUserCommand
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}