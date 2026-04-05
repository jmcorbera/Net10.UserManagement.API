using MediatR;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Repositories;
using Net10.UserManagement.Application.Common.Profiles;

namespace Net10.UserManagement.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        return user is not null ? UserProfile.UserMapToUserDto(user) : null;
    }
}