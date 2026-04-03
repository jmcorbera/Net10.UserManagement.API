using FluentAssertions;
using Moq;
using Net10.UserManagement.Application.Services;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Map_Users_To_UserDtos()
    {
        var createdAt = new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc);
        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = createdAt
            }
        };

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var service = new UserService(repositoryMock.Object);

        var result = (await service.GetAllAsync()).ToList();

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
            .ReturnsAsync(Array.Empty<User>());

        var service = new UserService(repositoryMock.Object);

        var result = await service.GetAllAsync();

        result.Should().BeEmpty();
    }
}
