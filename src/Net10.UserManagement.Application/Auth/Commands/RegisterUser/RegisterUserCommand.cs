using MediatR;
using Net10.UserManagement.Application.Auth.Models;

namespace Net10.UserManagement.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Identification, string Email, string FirstName, string LastName) : IRequest<RegisterUserResponse?>;

