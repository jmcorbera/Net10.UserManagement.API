namespace Net10.UserManagement.Application.Auth.Models;

public class RegisterUserResponse
{
    public Guid Id { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string IdentificationType { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }    
}