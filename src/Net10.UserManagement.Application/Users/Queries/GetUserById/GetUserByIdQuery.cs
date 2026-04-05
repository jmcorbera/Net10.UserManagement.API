using MediatR;
using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;