using FluentAssertions;
using Net10.UserManagement.Domain.Entities;

namespace Net10.UserManagement.Domain.Tests.Entities;

public class UserOtpTests
{
    [Fact]
    public void Create_Should_Create_UserOtp_With_Valid_Code()
    {
        var userId = Guid.NewGuid();
        var code = "123456";
        var expirationMinutes = 10;

        var userOtp = UserOtp.Create(userId, code, expirationMinutes);

        userOtp.Should().NotBeNull();
        userOtp.Id.Should().NotBe(Guid.Empty);
        userOtp.UserId.Should().Be(userId);
        userOtp.Code.Should().Be(code);
        userOtp.IsUsed.Should().BeFalse();
        userOtp.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        userOtp.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(expirationMinutes), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_Should_Use_Default_Expiration_When_Not_Specified()
    {
        var userId = Guid.NewGuid();
        var code = "123456";

        var userOtp = UserOtp.Create(userId, code);

        userOtp.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(10), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_Should_Throw_When_Code_Is_Empty()
    {
        var userId = Guid.NewGuid();

        var act = () => UserOtp.Create(userId, "");

        act.Should().Throw<ArgumentException>()
            .WithMessage("OTP code cannot be empty*");
    }

    [Fact]
    public void Create_Should_Throw_When_Code_Is_Null()
    {
        var userId = Guid.NewGuid();

        var act = () => UserOtp.Create(userId, null!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("OTP code cannot be empty*");
    }

    [Fact]
    public void Create_Should_Throw_When_Code_Is_Whitespace()
    {
        var userId = Guid.NewGuid();

        var act = () => UserOtp.Create(userId, "   ");

        act.Should().Throw<ArgumentException>()
            .WithMessage("OTP code cannot be empty*");
    }

    [Fact]
    public void Create_Should_Throw_When_Code_Is_Not_6_Digits()
    {
        var userId = Guid.NewGuid();

        var act1 = () => UserOtp.Create(userId, "12345");
        var act2 = () => UserOtp.Create(userId, "1234567");

        act1.Should().Throw<ArgumentException>()
            .WithMessage("OTP code must be 6 digits*");
        act2.Should().Throw<ArgumentException>()
            .WithMessage("OTP code must be 6 digits*");
    }

    [Fact]
    public void Create_Should_Throw_When_Code_Contains_Non_Digits()
    {
        var userId = Guid.NewGuid();

        var act1 = () => UserOtp.Create(userId, "12345a");
        var act2 = () => UserOtp.Create(userId, "ABC123");
        var act3 = () => UserOtp.Create(userId, "12-456");

        act1.Should().Throw<ArgumentException>()
            .WithMessage("OTP code must be 6 digits*");
        act2.Should().Throw<ArgumentException>()
            .WithMessage("OTP code must be 6 digits*");
        act3.Should().Throw<ArgumentException>()
            .WithMessage("OTP code must be 6 digits*");
    }

    [Fact]
    public void MarkAsUsed_Should_Mark_OTP_As_Used()
    {
        var userId = Guid.NewGuid();
        var userOtp = UserOtp.Create(userId, "123456", 10);

        userOtp.MarkAsUsed();

        userOtp.IsUsed.Should().BeTrue();
    }

    [Fact]
    public void MarkAsUsed_Should_Throw_When_Already_Used()
    {
        var userId = Guid.NewGuid();
        var userOtp = UserOtp.Create(userId, "123456", 10);
        userOtp.MarkAsUsed();

        var act = () => userOtp.MarkAsUsed();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("OTP has already been used");
    }

    [Fact]
    public void MarkAsUsed_Should_Throw_When_Expired()
    {
        var userId = Guid.NewGuid();
        var userOtp = UserOtp.Create(userId, "123456", -1);

        var act = () => userOtp.MarkAsUsed();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("OTP has expired");
    }

    [Fact]
    public void IsValid_Should_Return_True_When_Not_Used_And_Not_Expired()
    {
        var userId = Guid.NewGuid();
        var userOtp = UserOtp.Create(userId, "123456", 10);

        var result = userOtp.IsValid();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Used()
    {
        var userId = Guid.NewGuid();
        var userOtp = UserOtp.Create(userId, "123456", 10);
        userOtp.MarkAsUsed();

        var result = userOtp.IsValid();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Expired()
    {
        var userId = Guid.NewGuid();
        var userOtp = UserOtp.Create(userId, "123456", -1);

        var result = userOtp.IsValid();

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCode_Should_Return_True_When_Code_Matches_And_Valid()
    {
        var userId = Guid.NewGuid();
        var code = "123456";
        var userOtp = UserOtp.Create(userId, code, 10);

        var result = userOtp.ValidateCode(code);

        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCode_Should_Return_False_When_Code_Does_Not_Match()
    {
        var userId = Guid.NewGuid();
        var userOtp = UserOtp.Create(userId, "123456", 10);

        var result = userOtp.ValidateCode("654321");

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCode_Should_Return_False_When_OTP_Is_Used()
    {
        var userId = Guid.NewGuid();
        var code = "123456";
        var userOtp = UserOtp.Create(userId, code, 10);
        userOtp.MarkAsUsed();

        var result = userOtp.ValidateCode(code);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCode_Should_Return_False_When_OTP_Is_Expired()
    {
        var userId = Guid.NewGuid();
        var code = "123456";
        var userOtp = UserOtp.Create(userId, code, -1);

        var result = userOtp.ValidateCode(code);

        result.Should().BeFalse();
    }
}
