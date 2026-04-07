using FluentAssertions;
using Moq;
using AutoMapper;
using Net10.UserManagement.Application.Users.Queries.GetUserById;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Users.Queries.GetUserById;

public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task GetByIdAsync_Should_Return_UserResponse_When_User_Exists()
    {
        var user = User.CreatePending("12345678", "test@example.com", "John", "Doe");
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(m => m.Map<UserResponse>(It.IsAny<User>()))
            .Returns((User u) => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CreatedAt = u.CreatedAt
            });

        var handler = new GetUserByIdQueryHandler(repositoryMock.Object, mapperMock.Object);
        
        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);
        
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var mapperMock = new Mock<IMapper>();

        var handler = new GetUserByIdQueryHandler(repositoryMock.Object, mapperMock.Object);
        
        var result = await handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);
        
        result.Should().BeNull();
    }
}
