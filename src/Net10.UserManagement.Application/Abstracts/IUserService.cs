using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Abstracts;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> CreateAsync(UserCreateCommand user, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateAsync(Guid id, UserUpdateCommand user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
