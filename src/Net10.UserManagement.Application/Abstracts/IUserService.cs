using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Abstracts;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
}
