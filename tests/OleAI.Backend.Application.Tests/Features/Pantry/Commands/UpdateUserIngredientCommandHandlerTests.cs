using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.UpdateUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands;

public class UpdateUserIngredientCommandHandlerTests
{
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly Mock<IMediator> _mockMediator;
    private readonly PantryBusinessRules _businessRules;
    private readonly UpdateUserIngredientCommandHandler _handler;

    public UpdateUserIngredientCommandHandlerTests()
    {
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _mockMediator = new Mock<IMediator>();

        _businessRules = new PantryBusinessRules(
            _mockIngredientRepository.Object,
            _mockUserIngredientRepository.Object,
            _mockIngredientRequestRepository.Object);

        _handler = new UpdateUserIngredientCommandHandler(
            _mockUserIngredientRepository.Object,
            _businessRules,
            _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WhenInventoryItemNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateUserIngredientCommand
        {
            Request = new UpdateUserIngredientRequest { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), NewQuantity = 20 }
        };

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserIngredient?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(PantryMessages.InventoryItemNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenInventoryItemExists_ShouldUpdateQuantityAndReturnSuccessMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var inventoryId = Guid.NewGuid();
        var existing = UserIngredient.CreateStandard(inventoryId, userId, Guid.NewGuid(), 3, MeasurementUnit.Gram);

        var command = new UpdateUserIngredientCommand
        {
            Request = new UpdateUserIngredientRequest { Id = inventoryId, UserId = userId, NewQuantity = 7.5 }
        };

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _mockUserIngredientRepository
            .Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.IngredientQuantityUpdatedMessage);
        existing.Amount.Should().BeApproximately(7.5d, 0.0001d);

        _mockUserIngredientRepository.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUpdatedAmountIsNearZero_AndIngredientExists_ShouldDelete()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var inventoryId = Guid.NewGuid();
        var existing = UserIngredient.CreateStandard(inventoryId, userId, ingredientId, 3, MeasurementUnit.Gram);

        var command = new UpdateUserIngredientCommand
        {
            Request = new UpdateUserIngredientRequest
            {
                Id = inventoryId,
                UserId = userId,
                NewQuantity = 0.00005,
                NewUnit = MeasurementUnit.Gram
            }
        };

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.IngredientDepletedAndRemovedMessage);

        _mockUserIngredientRepository.Verify(
            r => r.DeleteAsync(existing, It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUserIngredientRepository.Verify(
            r => r.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mockMediator.Verify(
            m => m.Publish(
                It.IsAny<IngredientDepletedEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
