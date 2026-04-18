using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Net10.UserManagement.Application.Auth.Commands.RegisterUser;
using Net10.UserManagement.Application.Auth.Models;
using System.Reflection;

namespace Net10.UserManagement.Api.Tests.Endpoints;

public class AuthTests
{
    private readonly Mock<IMediator> _mediatorMock;

    public AuthTests()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public async Task Register_Should_Return_Created_When_User_Is_Registered_Successfully()
    {
        var command = new RegisterUserCommand(
            "12345678",
            1,
            "john.doe@example.com",
            "John",
            "Doe"
        );

        var createdUser = new RegisterUserResponse
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            CreatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(x => x.Send(It.Is<RegisterUserCommand>(c => 
                c.Identification == command.Identification && 
                c.Email == command.Email &&
                c.FirstName == command.FirstName &&
                c.LastName == command.LastName), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        var result = await InvokePrivateMethod<IResult>("Register", command, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<Created<RegisterUserResponse>>();
        var createdResult = result as Created<RegisterUserResponse>;
        createdResult!.Location.Should().Be($"/api/v1/users/{createdUser.Id}");
        createdResult.Value.Should().BeEquivalentTo(createdUser);
    }

    [Fact]
    public async Task Register_Should_Return_BadRequest_When_Registration_Fails()
    {
        var command = new RegisterUserCommand(
            "12345678",
            1,
            "john.doe@example.com",
            "John",
            "Doe"
        );

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisterUserResponse?)null);

        var result = await InvokePrivateMethod<IResult>("Register", command, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<BadRequest>();
    }

    [Fact]
    public async Task Register_Should_Call_Mediator_With_Correct_Command()
    {
        var command = new RegisterUserCommand(
            "87654321",
            1,
            "jane.doe@example.com",
            "Jane",
            "Doe"
        );

        var createdUser = new RegisterUserResponse
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            CreatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        await InvokePrivateMethod<IResult>("Register", command, _mediatorMock.Object, CancellationToken.None);

        _mediatorMock.Verify(
            x => x.Send(It.Is<RegisterUserCommand>(c => 
                c.Identification == command.Identification &&
                c.Email == command.Email &&
                c.FirstName == command.FirstName &&
                c.LastName == command.LastName), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Register_Should_Return_Created_With_Correct_Location_Header()
    {
        var command = new RegisterUserCommand(
            "12345678",
            1,
            "test@example.com",
            "Test",
            "User"
        );

        var userId = Guid.NewGuid();
        var createdUser = new RegisterUserResponse
        {
            Id = userId,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            CreatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        var result = await InvokePrivateMethod<IResult>("Register", command, _mediatorMock.Object, CancellationToken.None);

        result.Should().BeOfType<Created<RegisterUserResponse>>();
        var createdResult = result as Created<RegisterUserResponse>;
        createdResult!.Location.Should().Be($"/api/v1/users/{userId}");
    }

    private static async Task<T> InvokePrivateMethod<T>(string methodName, params object[] parameters)
    {
        var method = typeof(Api.Endpoints.Auth).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static) 
            ?? throw new InvalidOperationException($"Method {methodName} not found");

        var result = method.Invoke(null, parameters);
        if (result is Task<T> task)
            return await task;

        throw new InvalidOperationException($"Method {methodName} did not return Task<{typeof(T).Name}>");
    }
}
