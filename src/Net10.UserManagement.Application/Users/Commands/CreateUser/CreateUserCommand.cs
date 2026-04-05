using MediatR;
using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string FirstName, string LastName) : IRequest<UserResponse?>;
