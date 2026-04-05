using MediatR;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Repositories;
using AutoMapper;

namespace Net10.UserManagement.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users is not null ? users.Select(mapper.Map<UserResponse>) : [];
    }
}