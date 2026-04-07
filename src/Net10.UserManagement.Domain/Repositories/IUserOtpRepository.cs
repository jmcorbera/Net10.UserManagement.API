using Net10.UserManagement.Domain.Entities;

namespace Net10.UserManagement.Domain.Repositories;

public interface IUserOtpRepository
{
    Task SaveAsync(UserOtp otp, CancellationToken cancellationToken = default);
}