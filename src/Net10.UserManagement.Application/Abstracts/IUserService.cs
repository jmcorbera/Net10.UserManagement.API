using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Application.Users.Commands.CreateUser;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;

namespace Net10.UserManagement.Application.Abstracts;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse?> CreateAsync(CreateUserCommand user, CancellationToken cancellationToken = default);
    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserCommand user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
