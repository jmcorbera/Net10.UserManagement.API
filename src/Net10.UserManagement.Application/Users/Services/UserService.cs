using Net10.UserManagement.Application.Abstracts;
using Net10.UserManagement.Domain.Repositories;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;

namespace Net10.UserManagement.Application.Users.Services;

public class UserService (IUserRepository userRepository) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users is not null ? users.Select(UserMapToUserDto) : [];
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        return user is not null ? UserMapToUserDto(user) : null;
    }

    public async Task<UserDto?> CreateAsync(UserCreateCommand user, CancellationToken cancellationToken = default)
    {
        var newUser = User.CreatePending(user.Email, user.FirstName, user.LastName);
        var result = await userRepository.CreateAsync(newUser, cancellationToken);

        return result is not null ? UserMapToUserDto(result) : null;
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UserUpdateCommand user, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetByIdAsync(id, cancellationToken);
        if (existingUser == null)
            return null;
        
        existingUser.UpdateEmail(user.Email);
        var result = await userRepository.UpdateAsync(existingUser, cancellationToken);

        return result is not null ? UserMapToUserDto(result) : null;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {         
        await userRepository.DeleteAsync(id, cancellationToken);
    }
    
    private static UserDto UserMapToUserDto(User u)
    {
        return new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            CreatedAt = u.CreatedAt
        };
    }
}