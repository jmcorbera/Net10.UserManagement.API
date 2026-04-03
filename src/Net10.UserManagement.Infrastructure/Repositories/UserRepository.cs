using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Enumerable.Range(1, 5).Select(index => User.CreatePending(
            $"{Names[index - 1].Item1.ToLower()}.{Names[index - 1].Item2.ToLower()}@example.com",
            Names[index - 1].Item1,
            Names[index - 1].Item2
        )).ToArray());
    }

    private static readonly (string, string)[] Names =
    [
        ("John", "Doe"), ("Jane", "Smith"), ("Bob", "Johnson"), ("Alice", "Williams"), ("Charlie", "Brown"),
        ("David", "Jones"), ("Eve", "Garcia"), ("Frank", "Miller"), ("Grace", "Davis"), ("Henry", "Rodriguez")
    ];
}
