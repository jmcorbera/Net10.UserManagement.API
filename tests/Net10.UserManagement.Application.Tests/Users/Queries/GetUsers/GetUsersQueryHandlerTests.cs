using FluentAssertions;
using Moq;
using AutoMapper;
using Net10.UserManagement.Application.Users.Queries.GetUsers;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Users.Queries.GetUsers;

public class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task GetAllAsync_Should_Map_Users_To_UserResponse()
    {
        var users = new[]
        {
            User.CreatePending("12345678", 1, "john.doe@example.com", "John", "Doe")
        };

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

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

        var handler = new GetUsersQueryHandler(repositoryMock.Object, mapperMock.Object);

        var result = (await handler.Handle(new GetUsersQuery(), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(users[0].Id);
        result[0].Email.Should().Be(users[0].Email);
        result[0].FirstName.Should().Be(users[0].FirstName);
        result[0].LastName.Should().Be(users[0].LastName);
        result[0].CreatedAt.Should().Be(users[0].CreatedAt);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_Repository_Returns_Empty()
    {
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var mapperMock = new Mock<IMapper>();

        var handler = new GetUsersQueryHandler(repositoryMock.Object, mapperMock.Object);
    
        var result = (await handler.Handle(new GetUsersQuery(), CancellationToken.None)).ToList();

        result.Should().BeEmpty();
    }
}
