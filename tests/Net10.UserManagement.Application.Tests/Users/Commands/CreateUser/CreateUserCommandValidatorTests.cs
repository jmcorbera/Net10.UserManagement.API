using FluentAssertions;
using FluentValidation.TestHelper;
using Net10.UserManagement.Application.Users.Commands.CreateUser;

namespace Net10.UserManagement.Application.Tests.Users.Commands.CreateUser;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new CreateUserCommand("", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new CreateUserCommand("invalid-email", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is invalid");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_Email_Is_Valid()
    {
        var command = new CreateUserCommand("john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new CreateUserCommand("john.doe@example.com", "", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_FirstName_Is_Valid()
    {
        var command = new CreateUserCommand("john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new CreateUserCommand("john.doe@example.com", "John", "");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_LastName_Is_Valid()
    {
        var command = new CreateUserCommand("john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new CreateUserCommand("john.doe@example.com", "John", "Doe");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Have_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        var command = new CreateUserCommand("", "", "");
        
        var result = await _validator.TestValidateAsync(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}
