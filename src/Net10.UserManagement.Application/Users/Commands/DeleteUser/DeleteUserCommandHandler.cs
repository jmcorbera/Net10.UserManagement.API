using MediatR;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await userRepository.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}