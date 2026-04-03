using Net10.UserManagement.Application.Abstracts;
using Net10.UserManagement.Domain.Repositories;
using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Users.Services;

public class UserService (IUserRepository userRepository) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return (await userRepository.GetAllAsync()).Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            CreatedAt = u.CreatedAt
        });
    }
}
