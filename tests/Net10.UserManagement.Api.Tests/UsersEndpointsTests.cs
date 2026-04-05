using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Net10.UserManagement.Api.Endpoints;
using Net10.UserManagement.Application.Abstracts;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Application.Users.Commands.CreateUser;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;

using System.Reflection;

namespace Net10.UserManagement.Api.Tests;

public class UsersEndpointsTests
{
    private readonly Mock<IUserService> _userServiceMock;

    public UsersEndpointsTests()
    {
        _userServiceMock = new Mock<IUserService>();
    }

    [Fact]
    public async Task GetUsers_Should_Return_Ok_When_Users_Exist()
    {
        var users = new List<UserResponse>
        {
            new UserResponse
            {
                Id = Guid.NewGuid(),
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow
            },
            new UserResponse
            {
                Id = Guid.NewGuid(),
                Email = "jane.doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow
            }
        };

        _userServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await InvokePrivateMethod<IResult>("GetUsers", _userServiceMock.Object);

        result.Should().BeOfType<Ok<IEnumerable<UserResponse>>>();
        var okResult = result as Ok<IEnumerable<UserResponse>>;
        okResult!.Value.Should().HaveCount(2);
        okResult.Value.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetUsers_Should_Return_NotFound_When_No_Users_Exist()
    {
        _userServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserResponse>());

        var result = await InvokePrivateMethod<IResult>("GetUsers", _userServiceMock.Object);

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task GetUsers_Should_Return_NotFound_When_Users_Is_Null()
    {
        _userServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<UserResponse>)null!);

        var result = await InvokePrivateMethod<IResult>("GetUsers", _userServiceMock.Object);

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task GetUserById_Should_Return_Ok_When_User_Exists()
    {
        var userId = Guid.NewGuid();
        var user = new UserResponse
        {
            Id = userId,
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow
        };

        _userServiceMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await InvokePrivateMethod<IResult>("GetUserById", _userServiceMock.Object, userId);

        result.Should().BeOfType<Ok<UserResponse>>();
        var okResult = result as Ok<UserResponse>;
        okResult!.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetUserById_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();

        _userServiceMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse)null!);

        var result = await InvokePrivateMethod<IResult>("GetUserById", _userServiceMock.Object, userId);

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task CreateUser_Should_Return_Created_When_User_Is_Created_Successfully()
    {
        var createCommand = new CreateUserCommand
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        var createdUser = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = createCommand.Email,
            FirstName = createCommand.FirstName,
            LastName = createCommand.LastName,
            CreatedAt = DateTime.UtcNow
        };

        _userServiceMock
            .Setup(x => x.CreateAsync(createCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        var result = await InvokePrivateMethod<IResult>("CreateUser", _userServiceMock.Object, createCommand);

        result.Should().BeOfType<Created<UserResponse>>();
        var createdResult = result as Created<UserResponse>;
        createdResult!.Location.Should().Be($"/api/v1/users/{createdUser.Id}");
        createdResult.Value.Should().BeEquivalentTo(createdUser);
    }

    [Fact]
    public async Task CreateUser_Should_Return_BadRequest_When_User_Creation_Fails()
    {
        var createCommand = new CreateUserCommand
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _userServiceMock
            .Setup(x => x.CreateAsync(createCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse)null!);

        var result = await InvokePrivateMethod<IResult>("CreateUser", _userServiceMock.Object, createCommand);

        result.Should().BeOfType<BadRequest>();
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Ok_When_User_Is_Updated_Successfully()
    {
        var userId = Guid.NewGuid();
        var updateCommand = new UpdateUserCommand
        {
            Email = "john.updated@example.com"
        };

        var updatedUser = new UserResponse
        {
            Id = userId,
            Email = updateCommand.Email,
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow
        };

        _userServiceMock
            .Setup(x => x.UpdateAsync(userId, updateCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedUser);

        var result = await InvokePrivateMethod<IResult>("UpdateUser", _userServiceMock.Object, userId, updateCommand);

        result.Should().BeOfType<Ok<UserResponse>>();
        var okResult = result as Ok<UserResponse>;
        okResult!.Value.Should().BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();
        var updateCommand = new UpdateUserCommand
        {
            Email = "john.updated@example.com"
        };

        _userServiceMock
            .Setup(x => x.UpdateAsync(userId, updateCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse)null!);

        var result = await InvokePrivateMethod<IResult>("UpdateUser", _userServiceMock.Object, userId, updateCommand);

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task DeleteUser_Should_Return_NoContent()
    {
        var userId = Guid.NewGuid();

        _userServiceMock
            .Setup(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await InvokePrivateMethod<IResult>("DeleteUser", _userServiceMock.Object, userId);

        result.Should().BeOfType<NoContent>();
        _userServiceMock.Verify(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static async Task<T> InvokePrivateMethod<T>(string methodName, params object[] parameters)
    {
        var method = typeof(Users).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
            throw new InvalidOperationException($"Method {methodName} not found");

        var result = method.Invoke(null, parameters);
        if (result is Task<T> task)
            return await task;

        throw new InvalidOperationException($"Method {methodName} did not return Task<{typeof(T).Name}>");
    }
}
