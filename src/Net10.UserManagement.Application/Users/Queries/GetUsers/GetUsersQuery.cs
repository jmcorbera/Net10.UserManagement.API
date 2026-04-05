using MediatR;
using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<IEnumerable<UserResponse>>;