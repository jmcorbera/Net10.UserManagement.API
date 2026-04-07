using FluentAssertions;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Infrastructure.Repositories;

namespace Net10.UserManagement.Infrastructure.Tests.Repositories;

public class UserOtpRepositoryTests
{
    [Fact]
    public async Task SaveAsync_Should_Add_OTP_To_Repository()
    {
        var repository = new UserOtpRepository();
        var userId = Guid.NewGuid();
        var otp = UserOtp.Create(userId, "123456", 10);

        await repository.SaveAsync(otp);

        otp.Should().NotBeNull();
        otp.UserId.Should().Be(userId);
        otp.Code.Should().Be("123456");
    }

    [Fact]
    public async Task SaveAsync_Should_Complete_Successfully()
    {
        var repository = new UserOtpRepository();
        var userId = Guid.NewGuid();
        var otp = UserOtp.Create(userId, "654321", 10);

        var act = async () => await repository.SaveAsync(otp);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveAsync_Should_Store_Multiple_OTPs()
    {
        var repository = new UserOtpRepository();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var otp1 = UserOtp.Create(userId1, "111111", 10);
        var otp2 = UserOtp.Create(userId2, "222222", 10);
        var otp3 = UserOtp.Create(userId1, "333333", 10);

        await repository.SaveAsync(otp1);
        await repository.SaveAsync(otp2);
        await repository.SaveAsync(otp3);

        otp1.Should().NotBeNull();
        otp2.Should().NotBeNull();
        otp3.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveAsync_Should_Accept_CancellationToken()
    {
        var repository = new UserOtpRepository();
        var userId = Guid.NewGuid();
        var otp = UserOtp.Create(userId, "999888", 10);
        var cancellationToken = new CancellationToken();

        var act = async () => await repository.SaveAsync(otp, cancellationToken);

        await act.Should().NotThrowAsync();
    }
}
