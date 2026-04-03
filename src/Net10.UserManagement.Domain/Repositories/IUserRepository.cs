using Net10.UserManagement.Domain.Entities;

namespace Net10.UserManagement.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
}
