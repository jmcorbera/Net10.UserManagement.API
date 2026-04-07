using MediatR;
using Net10.UserManagement.Application.Auth.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;
using AutoMapper;
using Net10.UserManagement.Application.Common.Abstractions;

namespace Net10.UserManagement.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler(IUserRepository userRepository, IOtpGenerator otpGenerator, IUserOtpRepository userOtpRepository, IOtpSettingsProvider otpSettingsProvider, IEmailSender emailSender,  IMapper mapper) : IRequestHandler<RegisterUserCommand, RegisterUserResponse?>
{
    public async Task<RegisterUserResponse?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {       
        var newUser = User.CreatePending(request.Identification, request.Email, request.FirstName, request.LastName);
        var result = await userRepository.CreateAsync(newUser, cancellationToken) ?? throw new Exception("User creation failed");
        
        var otp = GenerateOtp(newUser.Id);
        await userOtpRepository.SaveAsync(otp, cancellationToken);

        await SendOtpEmailAsync(newUser, otp.Code, cancellationToken);
        
        return mapper.Map<RegisterUserResponse>(result);
    }

    private UserOtp GenerateOtp(Guid userId)
    {
        return UserOtp.Create(userId, otpGenerator.Generate(), otpSettingsProvider.ExpirationMinutes);
    }

    private async Task SendOtpEmailAsync(User user, string otp, CancellationToken cancellationToken)
    {
        var templateData = new { name = $"{user.FirstName} {user.LastName}", code = otp, ValidForMinutes = otpSettingsProvider.ExpirationMinutes };
        await emailSender.SendAsync(user.Email, "otpVerificationTemplate", templateData, cancellationToken);
    }
}