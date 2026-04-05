using MediatR;
using Net10.UserManagement.Application.Common.Profiles;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, UserResponse?>
{
    public async Task<UserResponse?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = User.CreatePending(request.Email, request.FirstName, request.LastName);
        var result = await userRepository.CreateAsync(newUser, cancellationToken);

        return result is not null ? UserProfile.UserMapToUserDto(result) : null;
    }
}