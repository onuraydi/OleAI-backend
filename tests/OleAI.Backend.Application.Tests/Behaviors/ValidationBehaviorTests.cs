using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using OleAI.Backend.Application.Behaviors;
using Xunit;
using ValidationException = OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types.ValidationException;

namespace OleAI.Backend.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNoValidatorsExist_ShouldCallNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest();
        var nextCalled = false;

        RequestHandlerDelegate<string> next = delegate
        {
            nextCalled = true;
            return Task.FromResult("Success");
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be("Success");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNext()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { mockValidator.Object });
        var request = new TestRequest();

        // Act
        var result = await behavior.Handle(request, delegate { return Task.FromResult("Success"); }, CancellationToken.None);

        // Assert
        result.Should().Be("Success");
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("FirstName", "Ad alanı boş bırakılamaz.")
        };

        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { mockValidator.Object });
        var request = new TestRequest();

        // Act
        var act = () => behavior.Handle(request, delegate { return Task.FromResult("Success"); }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Message.Contains("Ad alanı boş bırakılamaz."));
    }

    [Fact]
    public async Task Handle_WhenMultipleValidatorsFail_ShouldAggregateErrors()
    {
        // Arrange
        var validator1 = new Mock<IValidator<TestRequest>>();
        validator1.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("FirstName", "Hata 1") }));

        var validator2 = new Mock<IValidator<TestRequest>>();
        validator2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("LastName", "Hata 2") }));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator1.Object, validator2.Object });

        // Act
        var act = () => behavior.Handle(new TestRequest(), delegate { return Task.FromResult("Success"); }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Message.Contains("Hata 1") && ex.Message.Contains("Hata 2"));
    }

    // Test helper class
    public class TestRequest : IRequest<string>
    {
    }
}
