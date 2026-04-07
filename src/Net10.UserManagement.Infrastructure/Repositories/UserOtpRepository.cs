using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Infrastructure.Repositories;

public class UserOtpRepository : IUserOtpRepository
{
    private readonly List<UserOtp> _otps = [];

    public Task SaveAsync(UserOtp otp, CancellationToken cancellationToken = default)
    {
        _otps.Add(otp);
        return Task.CompletedTask;
    }
}