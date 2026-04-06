using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{

    private readonly List<User> _users = [];

    public UserRepository()
    {
        InitializeUsers();
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_users);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult<User?>(_users.FirstOrDefault(u => u.Id == id));
    }
    
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult<User?>(_users.FirstOrDefault(u => u.Email == email));
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Add(user);
        return await Task.FromResult(user);
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult<User?>(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var removedCount = _users.RemoveAll(u => u.Id == id);
        if (removedCount == 0)
            throw new InvalidOperationException("User not found");

        await Task.CompletedTask;
    }

    private void InitializeUsers()
    {
        _users.AddRange(Enumerable.Range(1, 5).Select(index => User.CreatePending(
            $"{Names[index - 1].Item1.ToLower()}.{Names[index - 1].Item2.ToLower()}@example.com",
            Names[index - 1].Item1,
            Names[index - 1].Item2
        )));
    }

    private static readonly (string, string)[] Names =
    [
        ("John", "Doe"), ("Jane", "Smith"), ("Bob", "Johnson"), ("Alice", "Williams"), ("Charlie", "Brown"),
        ("David", "Jones"), ("Eve", "Garcia"), ("Frank", "Miller"), ("Grace", "Davis"), ("Henry", "Rodriguez")
    ];
}
