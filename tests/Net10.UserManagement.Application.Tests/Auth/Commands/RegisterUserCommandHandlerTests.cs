using FluentAssertions;
using Moq;
using AutoMapper;
using Net10.UserManagement.Application.Auth.Commands.RegisterUser;
using Net10.UserManagement.Application.Auth.Models;
using Net10.UserManagement.Application.Common.Abstractions;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Auth.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOtpGenerator> _otpGeneratorMock;
    private readonly Mock<IUserOtpRepository> _userOtpRepositoryMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IOtpSettingsProvider> _otpSettingsProviderMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _otpGeneratorMock = new Mock<IOtpGenerator>();
        _userOtpRepositoryMock = new Mock<IUserOtpRepository>();
        _emailSenderMock = new Mock<IEmailSender>();
        _otpSettingsProviderMock = new Mock<IOtpSettingsProvider>();
        _mapperMock = new Mock<IMapper>();

        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object,
            _otpGeneratorMock.Object,
            _userOtpRepositoryMock.Object,
            _otpSettingsProviderMock.Object,
            _emailSenderMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Create_User_Successfully_When_User_Does_Not_Exist()
    {
        var command = new RegisterUserCommand(
            "12345678",
            "john.doe@example.com",
            "John",
            "Doe"
        );

        var expectedOtpCode = "123456";
        var createdUser = User.CreatePending(command.Identification, command.Email, command.FirstName, command.LastName);
        var expectedResponse = new RegisterUserResponse
        {
            Id = createdUser.Id,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            CreatedAt = createdUser.CreatedAt
        };

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        _otpGeneratorMock
            .Setup(o => o.Generate())
            .Returns(expectedOtpCode);

        _userOtpRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<UserOtp>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailSenderMock
            .Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<RegisterUserResponse>(It.IsAny<User>()))
            .Returns(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Email.Should().Be(command.Email);
        result.FirstName.Should().Be(command.FirstName);
        result.LastName.Should().Be(command.LastName);

        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _otpGeneratorMock.Verify(o => o.Generate(), Times.Once);
        _userOtpRepositoryMock.Verify(r => r.SaveAsync(It.IsAny<UserOtp>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailSenderMock.Verify(e => e.SendAsync(command.Email, "otpVerificationTemplate", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_User_Creation_Fails()
    {
        var command = new RegisterUserCommand(
            "12345678",
            "john.doe@example.com",
            "John",
            "Doe"
        );

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User creation failed");

        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Generate_And_Save_OTP_With_Correct_Expiration()
    {
        var command = new RegisterUserCommand(
            "12345678",
            "john.doe@example.com",
            "John",
            "Doe"
        );

        var expectedOtpCode = "654321";
        UserOtp? capturedOtp = null;
        var expirationMinutes = 10;

        _otpSettingsProviderMock
            .Setup(o => o.ExpirationMinutes)
            .Returns(expirationMinutes);

        _userRepositoryMock 
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        _otpGeneratorMock
            .Setup(o => o.Generate())
            .Returns(expectedOtpCode);

        _userOtpRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<UserOtp>(), It.IsAny<CancellationToken>()))
            .Callback<UserOtp, CancellationToken>((otp, _) => capturedOtp = otp)
            .Returns(Task.CompletedTask);

        _emailSenderMock
            .Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<RegisterUserResponse>(It.IsAny<User>()))
            .Returns(new RegisterUserResponse());

        await _handler.Handle(command, CancellationToken.None);

        capturedOtp.Should().NotBeNull();
        capturedOtp!.Code.Should().Be(expectedOtpCode);
        capturedOtp.IsUsed.Should().BeFalse();
        capturedOtp.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        capturedOtp.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(expirationMinutes), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_Should_Send_Email_With_Correct_Template_And_Data()
    {
        var command = new RegisterUserCommand(
            "12345678",
            "john.doe@example.com",
            "John",
            "Doe"
        );

        var expectedOtpCode = "999888";
        string? capturedEmail = null;
        string? capturedTemplate = null;
        object? capturedData = null;

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        _otpGeneratorMock
            .Setup(o => o.Generate())
            .Returns(expectedOtpCode);

        _userOtpRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<UserOtp>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailSenderMock
            .Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, object, CancellationToken>((email, template, data, _) =>
            {
                capturedEmail = email;
                capturedTemplate = template;
                capturedData = data;
            })
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<RegisterUserResponse>(It.IsAny<User>()))
            .Returns(new RegisterUserResponse());

        await _handler.Handle(command, CancellationToken.None);

        capturedEmail.Should().Be(command.Email);
        capturedTemplate.Should().Be("otpVerificationTemplate");
        capturedData.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_Map_User_To_Response_Correctly()
    {
        var command = new RegisterUserCommand(
            "12345678",
            "john.doe@example.com",
            "John",
            "Doe"
        );

        User? capturedUser = null;

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        _otpGeneratorMock
            .Setup(o => o.Generate())
            .Returns("123456");

        _userOtpRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<UserOtp>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailSenderMock
            .Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<RegisterUserResponse>(It.IsAny<User>()))
            .Callback<object>(u => capturedUser = u as User)
            .Returns(new RegisterUserResponse());

        await _handler.Handle(command, CancellationToken.None);

        _mapperMock.Verify(m => m.Map<RegisterUserResponse>(It.IsAny<User>()), Times.Once);
        capturedUser.Should().NotBeNull();
        capturedUser!.Identification.Should().Be(command.Identification);
        capturedUser.Email.Should().Be(command.Email);
        capturedUser.FirstName.Should().Be(command.FirstName);
        capturedUser.LastName.Should().Be(command.LastName);
    }
}
