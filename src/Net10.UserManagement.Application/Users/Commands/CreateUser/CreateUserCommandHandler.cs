using MediatR;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;
using AutoMapper;

namespace Net10.UserManagement.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<CreateUserCommand, UserResponse?>
{
    public async Task<UserResponse?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = User.CreatePending(request.Email, request.FirstName, request.LastName);
        var result = await userRepository.CreateAsync(newUser, cancellationToken);

        return result is not null ? mapper.Map<UserResponse>(result) : null;
    }
}