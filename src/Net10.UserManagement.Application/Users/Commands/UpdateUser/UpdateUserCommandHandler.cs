using MediatR;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Repositories;
using AutoMapper;

namespace Net10.UserManagement.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<UpdateUserCommand, UserResponse?>
{
    public async Task<UserResponse?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingUser == null)
            return null;
        
        existingUser.UpdateEmail(request.Email);
        var result = await userRepository.UpdateAsync(existingUser, cancellationToken);

        return result is not null ? mapper.Map<UserResponse>(result) : null;
    }
}