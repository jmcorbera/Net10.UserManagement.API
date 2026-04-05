using MediatR;

namespace Net10.UserManagement.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Unit>;
