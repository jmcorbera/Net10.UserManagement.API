using MediatR;
using Net10.UserManagement.Application.Users.Models;

namespace Net10.UserManagement.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    public Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}