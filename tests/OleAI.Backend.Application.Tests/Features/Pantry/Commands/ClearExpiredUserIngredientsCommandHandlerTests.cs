using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.ClearExpiredUserIngredients;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands;

public class ClearExpiredUserIngredientsCommandHandlerTests
{
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly ClearExpiredUserIngredientsCommandHandler _handler;

    public ClearExpiredUserIngredientsCommandHandlerTests()
    {
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _handler = new ClearExpiredUserIngredientsCommandHandler(_mockUserIngredientRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenNoExpiredItemsExist_ShouldReturnNoExpiredMessage()
    {
        // Arrange
        var command = new ClearExpiredUserIngredientsCommand { userId = Guid.NewGuid() };

        _mockUserIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<UserIngredient> { Items = new List<UserIngredient>() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(PantryMessages.NoExpiredProductsToCleanMessage);
        _mockUserIngredientRepository.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<UserIngredient>>(), false, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenExpiredItemsExist_ShouldDeleteRangeAndReturnCountMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiredItems = new List<UserIngredient>
        {
            UserIngredient.CreateStandard(Guid.NewGuid(), userId, Guid.NewGuid(), 1, DateTime.UtcNow.AddDays(-2)),
            UserIngredient.CreateStandard(Guid.NewGuid(), userId, Guid.NewGuid(), 3, DateTime.UtcNow.AddDays(-1))
        };

        var command = new ClearExpiredUserIngredientsCommand { userId = userId };

        _mockUserIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<UserIngredient> { Items = expiredItems });

        _mockUserIngredientRepository
            .Setup(r => r.DeleteRangeAsync(expiredItems, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredItems);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Tarihi geçmiş 2 adet ürün dolabınızdan temizlendi");
        _mockUserIngredientRepository.Verify(r => r.DeleteRangeAsync(expiredItems, false, It.IsAny<CancellationToken>()), Times.Once);
    }
}
