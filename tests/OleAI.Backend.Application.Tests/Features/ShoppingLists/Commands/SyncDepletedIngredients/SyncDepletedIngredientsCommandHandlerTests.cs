using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.ShoppingLists.Commands.SyncDepletedIngredients;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.Restock;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingLists.Commands.SyncDepletedIngredients;

public class SyncDepletedIngredientsCommandHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _shoppingListItemRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<ISmartRestockService> _smartRestockServiceMock;
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock;
    private readonly SyncDepletedIngredientsCommandHandler _handler;

    public SyncDepletedIngredientsCommandHandlerTests()
    {
        _shoppingListItemRepositoryMock = new Mock<IShoppingListItemRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _smartRestockServiceMock = new Mock<ISmartRestockService>();
        _ingredientRepositoryMock = new Mock<IIngredientRepository>();

        _handler = new SyncDepletedIngredientsCommandHandler(
            _shoppingListItemRepositoryMock.Object,
            _httpContextAccessorMock.Object,
            _smartRestockServiceMock.Object,
            _ingredientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var command = new SyncDepletedIngredientsCommand { ItemsToRestock = new List<DepletedIngredientRestockDto>() };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WhenNoItemsToRestock_ShouldReturnNoItemsMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);
        var command = new SyncDepletedIngredientsCommand { ItemsToRestock = new List<DepletedIngredientRestockDto>() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Hiçbir ürün eklenmedi.");
    }

    [Fact]
    public async Task Handle_WithValidItems_ShouldAddMissingItemsAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);

        var existingIngredientId = Guid.NewGuid();
        var newIngredientId = Guid.NewGuid();

        // 1 item already exists in shopping list
        var currentItems = new List<ShoppingListItem>
        {
            ShoppingListItem.CreateStandard(userId, existingIngredientId, 1, MeasurementUnit.Piece)
        };

        var paginatedResult = new Paginate<ShoppingListItem>
        {
            Items = currentItems
        };

        _shoppingListItemRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<ShoppingListItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var command = new SyncDepletedIngredientsCommand
        {
            ItemsToRestock = new List<DepletedIngredientRestockDto>
        {
            new DepletedIngredientRestockDto { IngredientId = existingIngredientId, Quantity = 5, Unit = (int)MeasurementUnit.Piece }, // Should be skipped
            new DepletedIngredientRestockDto { IngredientId = newIngredientId, Quantity = 2, Unit = (int)MeasurementUnit.Piece }, // Should be added
            new DepletedIngredientRestockDto { IngredientId = Guid.NewGuid(), Quantity = 0, Unit = (int)MeasurementUnit.Piece } // Should be skipped (Quantity == 0)
        }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("1 adet biten ürün alışveriş listesine başarıyla eklendi.");
        _shoppingListItemRepositoryMock.Verify(x => x.AddAsync(It.Is<ShoppingListItem>(i => i.IngredientId == newIngredientId), It.IsAny<CancellationToken>()), Times.Once);
        _shoppingListItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()), Times.Once); // Toplam 1 kere çağrılmalı
    }

    private void SetupUserContext(Guid userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }
}
