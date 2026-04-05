using FluentAssertions;
using Moq;
using Net10.UserManagement.Application.Users.Commands.CreateUser;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;
using Net10.UserManagement.Application.Users.Services;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Map_Users_To_UserDtos()
    {

        var users = new[]
        {
            User.CreatePending("john.doe@example.com", "John", "Doe")
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
            .ReturnsAsync([]);

        var service = new UserService(repositoryMock.Object);
    
        var result = await service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_UserDto_When_User_Exists()
    {
        // Arrange
        var user = User.CreatePending("test@example.com", "John", "Doe");
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var service = new UserService(repositoryMock.Object);
        
        // Act
        var result = await service.GetByIdAsync(user.Id);
        
        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var service = new UserService(repositoryMock.Object);
        
        // Act
        var result = await service.GetByIdAsync(userId);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_User_And_Return_UserDto()
    {
        // Arrange
        var createCommand = new CreateUserCommand
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);
        
        var service = new UserService(repositoryMock.Object);
        
        // Act
        var result = await service.CreateAsync(createCommand);
        
        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(createCommand.Email);
        result.FirstName.Should().Be(createCommand.FirstName);
        result.LastName.Should().Be(createCommand.LastName);
        repositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_User_Email_And_Return_UserDto()
    {
        // Arrange
        var user = User.CreatePending("old@example.com", "John", "Doe");
        var updateCommand = new UpdateUserCommand
        {
            Email = "new@example.com"
        };

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);
        
        var service = new UserService(repositoryMock.Object);
        
        // Act
        var result = await service.UpdateAsync(user.Id, updateCommand);
        
        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(updateCommand.Email);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateCommand = new UpdateUserCommand
        {
            Email = "new@example.com"
        };

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var service = new UserService(repositoryMock.Object);
        
        // Act
        var result = await service.UpdateAsync(userId, updateCommand);
        
        // Assert
        result.Should().BeNull();
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_Repository_DeleteAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var service = new UserService(repositoryMock.Object);
        
        // Act
        await service.DeleteAsync(userId);
        
        // Assert
        repositoryMock.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
