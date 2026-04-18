using FluentValidation.TestHelper;
using Moq;
using Net10.UserManagement.Application.Auth.Commands.RegisterUser;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Auth.Commands;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator(Mock.Of<IUserRepository>());
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new RegisterUserCommand("12345678", 1, "", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new RegisterUserCommand("12345678", 1, "invalid-email", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is invalid");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_Email_Is_Valid()
    {
        var command = new RegisterUserCommand("12345678", 1, "john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new RegisterUserCommand("12345678", 1, "john.doe@example.com", "", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_FirstName_Is_Valid()
    {
        var command = new RegisterUserCommand("12345678", 1, "john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new RegisterUserCommand("12345678", 1, "john.doe@example.com", "John", "");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_LastName_Is_Valid()
    {
        var command = new RegisterUserCommand("12345678", 1, "john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new RegisterUserCommand("12345678", 1, "john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Have_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        var command = new RegisterUserCommand("", 0, "", "", "");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}
