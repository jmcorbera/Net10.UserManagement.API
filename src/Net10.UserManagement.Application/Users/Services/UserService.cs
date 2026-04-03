using Net10.UserManagement.Application.Abstracts;
using Net10.UserManagement.Application.Models;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Services;

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
