using MediatR;
using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string Email) : IRequest<UserResponse>;
