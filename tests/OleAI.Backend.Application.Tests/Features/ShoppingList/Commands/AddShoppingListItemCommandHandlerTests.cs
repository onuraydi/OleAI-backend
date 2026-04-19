using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Application.Features.ShoppingList.Commands.AddShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands;

public class AddShoppingListItemCommandHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListRepo;
    private readonly Mock<IIngredientRepository> _mockIngredientRepo;
    private readonly Mock<ICacheService> _mockCache;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ShoppingListBusinessRules _rules;
    private readonly AddShoppingListItemCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _ingredientId = Guid.NewGuid();

    public AddShoppingListItemCommandHandlerTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepo = new Mock<IIngredientRepository>();
        _mockCache = new Mock<ICacheService>();
        _mockMapper = new Mock<IMapper>();

        _rules = new ShoppingListBusinessRules(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object);

        _handler = new AddShoppingListItemCommandHandler(
            _mockShoppingListRepo.Object,
            _mockIngredientRepo.Object,
            _mockCache.Object,
            _rules,
            _mockMapper.Object);
    }

    private void SetupIngredientExists(MeasurementCategory family = MeasurementCategory.Weight)
    {
        var ingredient = new Ingredient(_ingredientId, "Un", Guid.NewGuid(), family);

        _mockIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<Func<IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>?>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        _mockCache
            .Setup(c => c.GetAsync<IngredientCacheModel>(It.IsAny<string>()))
            .ReturnsAsync((IngredientCacheModel?)null);

        _mockMapper
            .Setup(m => m.Map<IngredientCacheModel>(It.IsAny<Ingredient>()))
            .Returns(new IngredientCacheModel { Id = ingredient.Id, Name = ingredient.Name, CategoryName = "Kategorisiz", UnitFamily = family });
    }

    private void SetupNoExistingListItem()
    {
        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldAddItemAndReturnResponse()
    {
        // Arrange
        SetupIngredientExists(MeasurementCategory.Weight);
        SetupNoExistingListItem();

        var addedItem = new ShoppingListItem(_userId, _ingredientId, 500, MeasurementUnit.Gram);
        _mockShoppingListRepo
            .Setup(r => r.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addedItem);

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem?)null);

        var command = new AddShoppingListItemCommand
        {
            Request = new AddShoppingListItemRequest
            {
                UserId = _userId,
                IngredientId = _ingredientId,
                Quantity = 500,
                Unit = MeasurementUnit.Gram
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IngredientName.Should().Be("Un");
        _mockShoppingListRepo.Verify(
            r => r.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotFound_ShouldThrowBusinessException()
    {
        // Arrange: ingredient hiç sistemde yok
        _mockIngredientRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                It.IsAny<Func<IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>?>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        _mockCache
            .Setup(c => c.GetAsync<IngredientCacheModel>(It.IsAny<string>()))
            .ReturnsAsync((IngredientCacheModel?)null);

        var command = new AddShoppingListItemCommand
        {
            Request = new AddShoppingListItemRequest
            {
                UserId = _userId,
                IngredientId = _ingredientId,
                Quantity = 1,
                Unit = MeasurementUnit.Piece
            }
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage(ShoppingListMessages.IngredientNotFound);
    }

    [Fact]
    public async Task Handle_WhenItemAlreadyInList_ShouldThrowBusinessException()
    {
        // Arrange: malzeme var ama zaten listede
        SetupIngredientExists();

        _mockShoppingListRepo
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<ShoppingListItem, bool>>>(),
                It.IsAny<Func<IQueryable<ShoppingListItem>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ShoppingListItem, object>>?>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShoppingListItem(_userId, _ingredientId, 1, MeasurementUnit.Gram));

        var command = new AddShoppingListItemCommand
        {
            Request = new AddShoppingListItemRequest
            {
                UserId = _userId,
                IngredientId = _ingredientId,
                Quantity = 1,
                Unit = MeasurementUnit.Gram
            }
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage(ShoppingListMessages.ProductAlreadyInList);
    }
}
