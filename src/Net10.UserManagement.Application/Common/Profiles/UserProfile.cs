using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using AutoMapper;

namespace Net10.UserManagement.Application.Common.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>();
    }
}