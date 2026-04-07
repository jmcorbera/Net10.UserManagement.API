using FluentValidation;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;
    
    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Identification)
            .NotEmpty().WithMessage("Identification is required")
            .Matches(@"^\d{7,8}$").WithMessage("Identification must be 7 or 8 digits")
            .MustAsync(BeUniqueIdentification)
            .WithMessage("Identification already exists");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid")
            .MustAsync(BeUniqueEmail)
            .WithMessage("Email already exists");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");
    }
    
    private async Task<bool> BeUniqueIdentification(string identification, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdentificationAsync(identification, cancellationToken) == null;
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByEmailAsync(email, cancellationToken) == null;
    }
}
