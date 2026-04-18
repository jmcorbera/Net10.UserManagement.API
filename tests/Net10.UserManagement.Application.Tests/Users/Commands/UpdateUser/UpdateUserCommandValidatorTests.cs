using FluentValidation.TestHelper;
using Moq;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Users.Commands.UpdateUser;

public class UpdateUserCommandValidatorTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UpdateUserCommandValidator _validator;

    public UpdateUserCommandValidatorTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _validator = new UpdateUserCommandValidator(_repositoryMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateUserCommand(Guid.Empty, "john.doe@example.com");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_Id_Is_Valid()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "john.doe@example.com");
        
        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "invalid-email");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is invalid");
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Already_Exists()
    {
        var existingUser = User.CreatePending("12345678", 1, "existing@example.com", "Existing", "User");
        var command = new UpdateUserCommand(Guid.NewGuid(), "existing@example.com");
        
        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email already exists");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_Email_Does_Not_Exist()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "new@example.com");
        
        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_All_Fields_Are_Valid_And_Email_Is_Unique()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "unique@example.com");
        
        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Call_Repository_GetByEmailAsync_When_Validating_Email()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "test@example.com");
        
        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        await _validator.TestValidateAsync(command);
        
        _repositoryMock.Verify(
            r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
