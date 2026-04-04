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

    public Task<UserDto?> GetByIdAsync(Guid id)
    {
        return Task.FromResult<UserDto?>(new UserDto
        {
            Id = id,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<UserDto?> CreateAsync(UserCommand user)
    {
        return Task.FromResult<UserDto?>(new UserDto
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<UserDto?> UpdateAsync(Guid id, UserCommand user)
    {
        return Task.FromResult<UserDto?>(new UserDto
        {
            Id = id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult<bool>(true);
    }
}