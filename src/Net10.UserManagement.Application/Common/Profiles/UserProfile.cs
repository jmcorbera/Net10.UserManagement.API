using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;

namespace Net10.UserManagement.Application.Common.Profiles;

public class UserProfile
{
    public static UserResponse UserMapToUserDto(User u)
    {
        return new UserResponse
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            CreatedAt = u.CreatedAt
        };
    }
}