using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.ResolveRequest;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands;

public class ResolveIngredientRequestCommandHandlerTests
{
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IIngredientAliasRepository> _mockIngredientAliasRepository;
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly ResolveIngredientRequestCommandHandler _handler;

    public ResolveIngredientRequestCommandHandlerTests()
    {
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockIngredientAliasRepository = new Mock<IIngredientAliasRepository>();
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();

        _handler = new ResolveIngredientRequestCommandHandler(
            _mockIngredientRequestRepository.Object,
            _mockIngredientRepository.Object,
            _mockIngredientAliasRepository.Object,
            _mockUserIngredientRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = CreateCommand(ResolutionAction.Reject);

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientRequest?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(PantryMessages.RequestNotFoundMessage);
    }

    [Fact]
    public async Task Handle_ApproveAsNew_WhenCategoryOrUnitMissing_ShouldThrowBusinessException()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Nohut Ezmesi");
        var command = CreateCommand(ResolutionAction.ApproveAsNew);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.NewCategoryId = null;
        command.Request.NewBaseUnitType = null;

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(PantryMessages.CategoryAndUnitTypeRequiredMessage);
    }

    [Fact]
    public async Task Handle_ApproveAsNew_WhenValid_ShouldAddIngredientResolveRequestAndUpdateInventory()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Acuka");
        var pendingInventoryItem = UserIngredient.CreatePending(Guid.NewGuid(), ingredientRequest.UserId, ingredientRequest.Id, 2);
        var categoryId = Guid.NewGuid();

        var command = CreateCommand(ResolutionAction.ApproveAsNew);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.NewCategoryId = categoryId;
        command.Request.NewBaseUnitType = "gram";

        Ingredient? addedIngredient = null;

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockIngredientRepository
            .Setup(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()))
            .Callback<Ingredient, CancellationToken>((ingredient, _) => addedIngredient = ingredient)
            .ReturnsAsync((Ingredient entity, CancellationToken _) => entity);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(ingredientRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingInventoryItem);

        _mockUserIngredientRepository
            .Setup(r => r.UpdateAsync(pendingInventoryItem, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingInventoryItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.RequestResolvedAndInventoryUpdatedMessage);
        ingredientRequest.Status.Should().Be(IngredientRequestStatus.ApprovedAsNew);
        addedIngredient.Should().NotBeNull();
        addedIngredient!.Name.Should().Be("Acuka");
        addedIngredient.BaseUnitType.Should().Be("gram");
        addedIngredient.CategoryId.Should().Be(categoryId);

        pendingInventoryItem.IngredientId.Should().Be(addedIngredient.Id);

        _mockIngredientRepository.Verify(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockIngredientRequestRepository.Verify(r => r.UpdateAsync(ingredientRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserIngredientRepository.Verify(r => r.UpdateAsync(pendingInventoryItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MergeAsAlias_WhenMasterIngredientMissing_ShouldThrowBusinessException()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Garcik");

        var command = CreateCommand(ResolutionAction.MergeAsAlias);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.MasterIngredientId = null;

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(PantryMessages.MasterMaterialRequiredForMergeMessage);
    }

    [Fact]
    public async Task Handle_MergeAsAlias_WhenValid_ShouldCreateAliasResolveRequestAndUpdateInventory()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Kırmızı Biber Tozu");
        var pendingInventoryItem = UserIngredient.CreatePending(Guid.NewGuid(), ingredientRequest.UserId, ingredientRequest.Id, 1);
        var masterIngredientId = Guid.NewGuid();

        var command = CreateCommand(ResolutionAction.MergeAsAlias);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.MasterIngredientId = masterIngredientId;
        command.Request.AdminNote = "Alias olarak birleştirildi.";

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockIngredientAliasRepository
            .Setup(r => r.AddAsync(It.IsAny<IngredientAlias>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientAlias entity, CancellationToken _) => entity);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(ingredientRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingInventoryItem);

        _mockUserIngredientRepository
            .Setup(r => r.UpdateAsync(pendingInventoryItem, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingInventoryItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.RequestResolvedAndInventoryUpdatedMessage);
        ingredientRequest.Status.Should().Be(IngredientRequestStatus.MergedWithAlias);
        ingredientRequest.AdminNote.Should().Be("Alias olarak birleştirildi.");
        pendingInventoryItem.IngredientId.Should().Be(masterIngredientId);

        _mockIngredientAliasRepository.Verify(r => r.AddAsync(
            It.Is<IngredientAlias>(a => a.IngredientId == masterIngredientId && a.AliasName == ingredientRequest.RequestedName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MergeAsAlias_WhenAdminNoteIsNull_ShouldUseDefaultMergedMessage()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Sumakkk");
        var masterIngredientId = Guid.NewGuid();

        var command = CreateCommand(ResolutionAction.MergeAsAlias);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.MasterIngredientId = masterIngredientId;
        command.Request.AdminNote = null;

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockIngredientAliasRepository
            .Setup(r => r.AddAsync(It.IsAny<IngredientAlias>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientAlias entity, CancellationToken _) => entity);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(ingredientRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserIngredient?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.RequestResolvedAndInventoryUpdatedMessage);
        ingredientRequest.Status.Should().Be(IngredientRequestStatus.MergedWithAlias);
        ingredientRequest.AdminNote.Should().Be(PantryMessages.MergedAsAliasMessage);
        _mockUserIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Reject_WhenValid_ShouldRejectAndReturnRejectMessage()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "123##");
        var command = CreateCommand(ResolutionAction.Reject);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.AdminNote = "Anlamsız giriş";

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(ingredientRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.RequestRejectedMessage);
        ingredientRequest.Status.Should().Be(IngredientRequestStatus.Rejected);
        ingredientRequest.AdminNote.Should().Be("Anlamsız giriş");

        _mockUserIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Reject_WhenAdminNoteIsNull_ShouldUseDefaultRejectMessage()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "###");
        var command = CreateCommand(ResolutionAction.Reject);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.AdminNote = null;

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(ingredientRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.RequestRejectedMessage);
        ingredientRequest.AdminNote.Should().Be(PantryMessages.RequestRejectedMessage);
    }

    [Fact]
    public async Task Handle_ApproveAsNew_WhenPendingInventoryNotFound_ShouldStillResolveRequest()
    {
        // Arrange
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Kimyonlu Tuz");
        var categoryId = Guid.NewGuid();

        var command = CreateCommand(ResolutionAction.ApproveAsNew);
        command.Request.RequestId = ingredientRequest.Id;
        command.Request.NewCategoryId = categoryId;
        command.Request.NewBaseUnitType = "gram";

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockIngredientRepository
            .Setup(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient entity, CancellationToken _) => entity);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(ingredientRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredientRequest);

        _mockUserIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserIngredient?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.RequestResolvedAndInventoryUpdatedMessage);
        ingredientRequest.Status.Should().Be(IngredientRequestStatus.ApprovedAsNew);
        _mockUserIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static ResolveIngredientRequestCommand CreateCommand(ResolutionAction action)
    {
        return new ResolveIngredientRequestCommand
        {
            Request = new ResolveIngredientRequestDto
            {
                RequestId = Guid.NewGuid(),
                Action = action
            }
        };
    }
}
