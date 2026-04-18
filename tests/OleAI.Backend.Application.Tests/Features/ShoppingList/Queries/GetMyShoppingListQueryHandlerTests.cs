using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Features.ShoppingList.Queries.GetMyShoppingList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Queries;

public class GetMyShoppingListQueryHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetMyShoppingListQueryHandler _handler;

    public GetMyShoppingListQueryHandlerTests()
    {
        _mockRepo = new Mock<IShoppingListItemRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetMyShoppingListQueryHandler(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedActiveItems()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();

        var items = new List<ShoppingListItem>
        {
            new ShoppingListItem(userId, ingredientId, 2, MeasurementUnit.Piece)
        };

        var paginate = new Paginate<ShoppingListItem>
        {
            Items = items,
            PageNumber = 0,
            PageSize = 100,
            TotalItems = 1,
            TotalPages = 1
        };

        _mockRepo
            .Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<ShoppingListItem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate);

        var expectedResponse = new List<ShoppingListItemResponse>
        {
            new() { Id = Guid.NewGuid(), IngredientId = ingredientId, Quantity = 2, Unit = MeasurementUnit.Piece }
        };
        _mockMapper
            .Setup(m => m.Map<List<ShoppingListItemResponse>>(items))
            .Returns(expectedResponse);

        var query = new GetMyShoppingListQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Quantity.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenNoItems_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<ShoppingListItem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<ShoppingListItem> { Items = new List<ShoppingListItem>() });

        _mockMapper
            .Setup(m => m.Map<List<ShoppingListItemResponse>>(It.IsAny<List<ShoppingListItem>>()))
            .Returns(new List<ShoppingListItemResponse>());

        var query = new GetMyShoppingListQuery { UserId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
