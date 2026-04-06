using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Net10.UserManagement.Application.Common.Behaviors;

namespace Net10.UserManagement.Application.Tests.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Should_Call_Next_When_No_Validators_Exist()
    {
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var expectedResponse = new TestResponse { Value = "Success" };
        
        Task<TestResponse> next(CancellationToken ct) => Task.FromResult(expectedResponse);
        
        var result = await behavior.Handle(request, next, CancellationToken.None);
        
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Should_Call_Next_When_Validation_Passes()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var expectedResponse = new TestResponse { Value = "Success" };
        
        Task<TestResponse> next(CancellationToken ct) => Task.FromResult(expectedResponse);
        
        var result = await behavior.Handle(request, next, CancellationToken.None);
        
        result.Should().Be(expectedResponse);
        validatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Validation_Fails()
    {
        var validationFailure = new ValidationFailure("Property", "Error message");
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([validationFailure]));
        
        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();

        static Task<TestResponse> next(CancellationToken ct) => Task.FromResult(new TestResponse());
        
        var act = async () => await behavior.Handle(request, next, CancellationToken.None);
        
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Should_Throw_ValidationException_With_All_Failures_When_Multiple_Validators_Fail()
    {
        var validationFailure1 = new ValidationFailure("Property1", "Error message 1");
        var validationFailure2 = new ValidationFailure("Property2", "Error message 2");
        
        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([validationFailure1]));
        
        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([validationFailure2]));
        
        var validators = new[] { validator1Mock.Object, validator2Mock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        
        static Task<TestResponse> next(CancellationToken ct) => Task.FromResult(new TestResponse());
        
        var act = async () => await behavior.Handle(request, next, CancellationToken.None);
        
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_Not_Call_Next_When_Validation_Fails()
    {
        var validationFailure = new ValidationFailure("Property", "Error message");
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([validationFailure]));
        
        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        
        var nextCalled = false;
        static Task<TestResponse> next(CancellationToken ct, bool nextCalled)
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse());
        }
        
        try
        {
            await behavior.Handle(request, ct => next(ct, nextCalled), CancellationToken.None);
        }
        catch (ValidationException)
        {
        }
        
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Pass_CancellationToken_To_Validators()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());
        
        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        
        static Task<TestResponse> next(CancellationToken ct) => Task.FromResult(new TestResponse());
        
        await behavior.Handle(request, next, cancellationToken);
        
        validatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), cancellationToken), Times.Once);
    }

    public class TestRequest : IRequest<TestResponse>{}

    public class TestResponse
    {
        public string Value { get; set; } = string.Empty;
    }
}
