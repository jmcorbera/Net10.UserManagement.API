using MediatR;
using Net10.UserManagement.Application.Common.Profiles;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserCommand, UserResponse?>
{
    public async Task<UserResponse?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingUser == null)
            return null;
        
        existingUser.UpdateEmail(request.Email);
        var result = await userRepository.UpdateAsync(existingUser, cancellationToken);

        return result is not null ? UserProfile.UserMapToUserDto(result) : null;
    }
}