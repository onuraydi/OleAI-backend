using System.Linq.Expressions;
using System.Threading;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Rules;

public class PantryBusinessRulesTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly PantryBusinessRules _rules;

    public PantryBusinessRulesTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _rules = new PantryBusinessRules(
            _mockIngredientRepository.Object,
            _mockUserIngredientRepository.Object,
            _mockIngredientRequestRepository.Object);
    }

    [Fact]
    public async Task IngredientMustExistWhenSelectedFromList_ShouldThrowBusinessException_WhenIngredientDoesNotExist()
    {
        // Arrange
        var ingredientId = Guid.NewGuid();
        _mockIngredientRepository.Setup(r => r.GetAsync(
            It.IsAny<Expression<Func<Ingredient, bool>>>(),
            null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient)null!);

        // Act
        Func<Task> action = async () => await _rules.IngredientMustExistWhenSelectedFromList(ingredientId);

        // Assert
        await action.Should().ThrowAsync<BusinessException>().WithMessage(PantryMessages.IngredientNotFoundMessage);
    }

    [Fact]
    public async Task IngredientMustExistWhenSelectedFromList_ShouldNotThrow_WhenIngredientExists()
    {
        // Arrange
        var ingredientId = Guid.NewGuid();
        _mockIngredientRepository.Setup(r => r.GetAsync(
            It.IsAny<Expression<Func<Ingredient, bool>>>(),
            null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(ingredientId, "Tomato", Guid.NewGuid(), "kg"));

        // Act
        Func<Task> action = async () => await _rules.IngredientMustExistWhenSelectedFromList(ingredientId);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public void CannotSendBothIdAndName_ShouldThrowBusinessException_WhenBothProvided()
    {
        // Act
        Action action = () => _rules.CannotSendBothIdAndName(Guid.NewGuid(), "Requested Name");

        // Assert
        action.Should().Throw<BusinessException>().WithMessage(PantryMessages.ProductSelectionConflictMessage);
    }

    [Fact]
    public void CannotSendBothIdAndName_ShouldThrowBusinessException_WhenNeitherProvided()
    {
        // Act
        Action action = () => _rules.CannotSendBothIdAndName(null, "");

        // Assert
        action.Should().Throw<BusinessException>().WithMessage(PantryMessages.ProductSelectionRequiredMessage);
    }

    [Fact]
    public async Task InventoryItemNotFound_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockUserIngredientRepository.Setup(r => r.GetAsync(
            It.IsAny<Expression<Func<UserIngredient, bool>>>(),
            null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserIngredient)null!);

        // Act
        Func<Task> action = async () => await _rules.InventoryItemNotFound(id);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage(PantryMessages.InventoryItemNotFoundMessage);
    }

    [Fact]
    public void IngredientRequestMustExistWhenSelected_ShouldThrowNotFoundException_WhenRequestIsNull()
    {
        // Act
        Action action = () => _rules.IngredientRequestMustExistWhenSelected(null!);

        // Assert
        action.Should().Throw<NotFoundException>().WithMessage(PantryMessages.IngredientRequestNotFoundMessage);
    }

    [Fact]
    public void CannotBeCancelledRequest_ShouldThrowBusinessException_WhenStatusIsNotPending()
    {
        // Act
        Action action = () => _rules.CannotBeCancelledRequest(IngredientRequestStatus.ApprovedAsNew);

        // Assert
        action.Should().Throw<BusinessException>().WithMessage(PantryMessages.RequestAlreadyProcessedCannotBeCancelledMessage);
    }

    [Fact]
    public void CannotBeCancelledRequest_ShouldNotThrow_WhenStatusIsPending()
    {
        // Act
        Action action = () => _rules.CannotBeCancelledRequest(IngredientRequestStatus.Pending);

        // Assert
        action.Should().NotThrow();
    }
}
