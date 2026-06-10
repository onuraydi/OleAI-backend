using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.CancelRequest;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands;

public class CancelIngredientRequestCommandHandlerTests
{
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly PantryBusinessRules _businessRules;
    private readonly CancelIngredientRequestCommandHandler _handler;

    public CancelIngredientRequestCommandHandlerTests()
    {
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();

        _businessRules = new PantryBusinessRules(
            _mockIngredientRepository.Object,
            _mockUserIngredientRepository.Object,
            _mockIngredientRequestRepository.Object);

        _handler = new CancelIngredientRequestCommandHandler(
            _mockIngredientRequestRepository.Object,
            _mockUserIngredientRepository.Object,
            _businessRules);
    }

    [Fact]
    public async Task Handle_WhenRequestNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CancelIngredientRequestCommand { RequestId = Guid.NewGuid(), UserId = Guid.NewGuid() };

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientRequest?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(PantryMessages.IngredientRequestNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenRequestAlreadyProcessed_ShouldThrowBusinessException()
    {
        // Arrange
        var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Domat");
        request.ResolveAsApproved();

        var command = new CancelIngredientRequestCommand { RequestId = request.Id, UserId = request.UserId };

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(PantryMessages.RequestAlreadyProcessedCannotBeCancelledMessage);

        _mockUserIngredientRepository.Verify(r => r.DeleteAsync(It.IsAny<UserIngredient>(), true, It.IsAny<CancellationToken>()), Times.Never);
        _mockIngredientRequestRepository.Verify(r => r.UpdateAsync(It.IsAny<IngredientRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPendingRequestExists_ShouldRejectAndDeletePendingInventory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestId = Guid.NewGuid();

        var request = new IngredientRequest(requestId, userId, "Sutto");
        var pendingInventoryItem = UserIngredient.CreatePending(Guid.NewGuid(), userId, requestId, 2, MeasurementUnit.Piece);

        var command = new CancelIngredientRequestCommand { RequestId = requestId, UserId = userId };

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingInventoryItem);

        _mockUserIngredientRepository
            .Setup(r => r.DeleteAsync(pendingInventoryItem, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingInventoryItem);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.IngredientRequestCancelledMessage);
        request.Status.Should().Be(IngredientRequestStatus.Rejected);
        request.AdminNote.Should().Be(PantryMessages.RequestCancelledByUserMessage);

        _mockUserIngredientRepository.Verify(r => r.DeleteAsync(pendingInventoryItem, true, It.IsAny<CancellationToken>()), Times.Once);
        _mockIngredientRequestRepository.Verify(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
