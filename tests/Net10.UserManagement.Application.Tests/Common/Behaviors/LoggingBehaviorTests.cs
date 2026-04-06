using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Net10.UserManagement.Application.Common.Behaviors;

namespace Net10.UserManagement.Application.Tests.Common.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> _loggerMock;
    private readonly LoggingBehavior<TestRequest, TestResponse> _behavior;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        _behavior = new LoggingBehavior<TestRequest, TestResponse>(_loggerMock.Object);
    }

    [Fact]
    public async Task Should_Call_Next_And_Return_Response()
    {
        var request = new TestRequest();
        var expectedResponse = new TestResponse { Value = "Success" };

        Task<TestResponse> next(CancellationToken ct) => Task.FromResult(expectedResponse);

        var result = await _behavior.Handle(request, next, CancellationToken.None);
        
        result.Should().Be(expectedResponse);
    }


    [Fact]
    public async Task Should_Rethrow_Exception_After_Logging()
    {
        var request = new TestRequest();
        var exception = new InvalidOperationException("Test exception");
        
        Task<TestResponse> next(CancellationToken ct) => throw exception;
        
        var act = async () => await _behavior.Handle(request, next, CancellationToken.None);
        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");
    }


    public class TestRequest : IRequest<TestResponse>{}

    public class TestResponse
    {
        public string Value { get; set; } = string.Empty;
    }
}
