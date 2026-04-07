using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;
using Net10.UserManagement.Application.Users.Commands.DeleteUser;
using Net10.UserManagement.Application.Users.Queries.GetUsers;
using Net10.UserManagement.Application.Users.Queries.GetUserById;

using System.Reflection;

namespace Net10.UserManagement.Api.Tests.Endpoints;

public class UsersTest
{
    private readonly Mock<IMediator> _mediatorMock;

    public UsersTest()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public async Task GetUsers_Should_Return_Ok_When_Users_Exist()
    {
        var users = new List<UserResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "jane.doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await InvokePrivateMethod<IResult>("GetUsers", _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<Ok<IEnumerable<UserResponse>>>();
        var okResult = result as Ok<IEnumerable<UserResponse>>;
        okResult!.Value.Should().HaveCount(2);
        okResult.Value.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetUsers_Should_Return_NotFound_When_No_Users_Exist()
    {
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await InvokePrivateMethod<IResult>("GetUsers", _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task GetUsers_Should_Return_NotFound_When_Users_Is_Null()
    {
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<UserResponse>)null!);

        var result = await InvokePrivateMethod<IResult>("GetUsers", _mediatorMock.Object, CancellationToken.None);

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

        _mediatorMock
            .Setup(x => x.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await InvokePrivateMethod<IResult>("GetUserById", userId, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<Ok<UserResponse>>();
        var okResult = result as Ok<UserResponse>;
        okResult!.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetUserById_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();

        _mediatorMock
            .Setup(x => x.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse)null!);

        var result = await InvokePrivateMethod<IResult>("GetUserById", userId, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<NotFound>();
    }

/*     [Fact]
    public async Task CreateUser_Should_Return_Created_When_User_Is_Created_Successfully()
    {
        var createCommand = new CreateUserCommand(
            "john.doe@example.com",
            "John",
            "Doe"
        );

        var createdUser = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = createCommand.Email,
            FirstName = createCommand.FirstName,
            LastName = createCommand.LastName,
            CreatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(x => x.Send(It.Is<CreateUserCommand>(c => c.Email == createCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        var result = await InvokePrivateMethod<IResult>("CreateUser", createCommand, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<Created<UserResponse>>();
        var createdResult = result as Created<UserResponse>;
        createdResult!.Location.Should().Be($"/api/v1/users/{createdUser.Id}");
        createdResult.Value.Should().BeEquivalentTo(createdUser);
    }

    [Fact]
    public async Task CreateUser_Should_Return_BadRequest_When_User_Creation_Fails() 
    {
        var createCommand = new CreateUserCommand(
            "john.doe@example.com",
            "John",
            "Doe"
        );

        _mediatorMock
            .Setup(x => x.Send(It.Is<CreateUserCommand>(c => c.Email == createCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse)null!);

        var result = await InvokePrivateMethod<IResult>("CreateUser", createCommand, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<BadRequest>();
    }*/

    [Fact]
    public async Task UpdateUser_Should_Return_Ok_When_User_Is_Updated_Successfully()
    {
        var userId = Guid.NewGuid();
        var updateCommand = new UpdateUserCommand(userId, "john.updated@example.com");

        var updatedUser = new UserResponse
        {
            Id = userId,
            Email = updateCommand.Email,
            FirstName = "John",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(x => x.Send(It.Is<UpdateUserCommand>(c => c.Id == userId && c.Email == updateCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedUser);

        var result = await InvokePrivateMethod<IResult>("UpdateUser", userId, updateCommand, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<Ok<UserResponse>>();
        var okResult = result as Ok<UserResponse>;
        okResult!.Value.Should().BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();
        var updateCommand = new UpdateUserCommand(userId, "john.updated@example.com");

        _mediatorMock
            .Setup(x => x.Send(It.Is<UpdateUserCommand>(c => c.Id == userId && c.Email == updateCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse)null!);

        var result = await InvokePrivateMethod<IResult>("UpdateUser", userId, updateCommand, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task DeleteUser_Should_Return_NoContent()
    {
        var userId = Guid.NewGuid();

        _mediatorMock
            .Setup(x => x.Send(It.Is<DeleteUserCommand>(c => c.Id == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await InvokePrivateMethod<IResult>("DeleteUser", userId, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<NoContent>();
        _mediatorMock.Verify(x => x.Send(It.Is<DeleteUserCommand>(c => c.Id == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static async Task<T> InvokePrivateMethod<T>(string methodName, params object[] parameters)
    {
        var method = typeof(Api.Endpoints.Users).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static) 
            ?? throw new InvalidOperationException($"Method {methodName} not found");

        var result = method.Invoke(null, parameters);
        if (result is Task<T> task)
            return await task;

        throw new InvalidOperationException($"Method {methodName} did not return Task<{typeof(T).Name}>");
    }
}
