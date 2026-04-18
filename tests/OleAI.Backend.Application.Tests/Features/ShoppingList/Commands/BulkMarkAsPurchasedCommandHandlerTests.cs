using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Commands.BulkMarkAsPurchased;
using OleAI.Backend.Application.Features.ShoppingList.Constants;
using OleAI.Backend.Application.Features.ShoppingList.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands;

public class BulkMarkAsPurchasedCommandHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListRepo;
    private readonly Mock<IMediator> _mockMediator;
    private readonly BulkMarkAsPurchasedCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public BulkMarkAsPurchasedCommandHandlerTests()
    {
        _mockShoppingListRepo = new Mock<IShoppingListItemRepository>();
        _mockMediator = new Mock<IMediator>();

        _handler = new BulkMarkAsPurchasedCommandHandler(
            _mockShoppingListRepo.Object,
            _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WhenValidItems_ShouldMarkAllAsPurchasedAndPublishEvents()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var item1 = new ShoppingListItem(_userId, Guid.NewGuid(), 1, MeasurementUnit.Piece);
        var item2 = new ShoppingListItem(_userId, Guid.NewGuid(), 500, MeasurementUnit.Gram);

        var paginate = new Paginate<ShoppingListItem>
        {
            Items = new List<ShoppingListItem> { item1, item2 }
        };

        _mockShoppingListRepo
            .Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<ShoppingListItem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate);

        _mockShoppingListRepo
            .Setup(r => r.UpdateRangeAsync(
                It.IsAny<IEnumerable<ShoppingListItem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingListItem> { item1, item2 });

        var command = new BulkMarkAsPurchasedCommand
        {
            UserId = _userId,
            ItemIds = new List<Guid> { id1, id2 }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: her iki item satın alındı
        item1.IsPurchased.Should().BeTrue();
        item2.IsPurchased.Should().BeTrue();

        // Assert: mesaj doğru
        result.Should().Be(string.Format(ShoppingListMessages.BulkMarkedAsPurchased, 2));

        // Assert: her item için event yayınlandı
        _mockMediator.Verify(
            m => m.Publish(It.IsAny<ItemPurchasedEvent>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        // Assert: UpdateRangeAsync bir kez çağrıldı
        _mockShoppingListRepo.Verify(
            r => r.UpdateRangeAsync(It.IsAny<IEnumerable<ShoppingListItem>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoItemsFound_ShouldThrowNotFoundException()
    {
        // Arrange: hiçbir item bulunamadı (yanlış userId veya zaten purchased)
        _mockShoppingListRepo
            .Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<ShoppingListItem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<ShoppingListItem> { Items = new List<ShoppingListItem>() });

        var command = new BulkMarkAsPurchasedCommand
        {
            UserId = _userId,
            ItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ShoppingListMessages.ItemNotFoundOrUnauthorized);
    }

    [Fact]
    public async Task Handle_WhenEmptyIdList_ShouldThrowBusinessException()
    {
        // Arrange
        var command = new BulkMarkAsPurchasedCommand
        {
            UserId = _userId,
            ItemIds = new List<Guid>()    // boş liste
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("En az bir ürün seçmelisiniz.");
    }
}
