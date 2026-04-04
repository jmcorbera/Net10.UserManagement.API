using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Abstracts;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> CreateAsync(UserCommand user);
    Task<UserDto?> UpdateAsync(Guid id, UserCommand user);
    Task<bool> DeleteAsync(Guid id);
}
