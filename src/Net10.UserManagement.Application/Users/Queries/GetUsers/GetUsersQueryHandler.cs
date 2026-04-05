using MediatR;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Repositories;
using Net10.UserManagement.Application.Common.Profiles;

namespace Net10.UserManagement.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users is not null ? users.Select(UserProfile.UserMapToUserDto) : [];
    }
}