using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.ClearExpiredUserIngredients;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands
{
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
        public async Task Handle_WhenExpiredItemsExist_ShouldDeleteThemAndReturnMessage()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ClearExpiredUserIngredientsCommand { userId = userId };

            var expiredItems = new List<UserIngredient>
            {
                UserIngredient.CreateStandard(Guid.NewGuid(), userId, Guid.NewGuid(), 1, DateTime.UtcNow.AddDays(-1)),
                UserIngredient.CreateStandard(Guid.NewGuid(), userId, Guid.NewGuid(), 2, DateTime.UtcNow.AddDays(-2))
            };

            var paginatedResult = new Paginate<UserIngredient> { Items = expiredItems };

            _mockUserIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResult);

            _mockUserIngredientRepository
                .Setup(r => r.DeleteRangeAsync(It.IsAny<IList<UserIngredient>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IList<UserIngredient>)null!); // Method signature might return Unit/Task

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be($"Tarihi geçmiş {expiredItems.Count} adet ürün dolabınızdan temizlendi");

            _mockUserIngredientRepository.Verify(r => r.GetListAsync(
                It.Is<QueryOptions<UserIngredient>>(opt => opt.EnableTracking == false), 
                It.IsAny<CancellationToken>()), Times.Once);

            _mockUserIngredientRepository.Verify(r => r.DeleteRangeAsync(expiredItems, false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoExpiredItemsExist_ShouldReturnNoExpiredProductsMessage()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ClearExpiredUserIngredientsCommand { userId = userId };

            var paginatedResult = new Paginate<UserIngredient> { Items = new List<UserIngredient>() };

            _mockUserIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(PantryMessages.NoExpiredProductsToCleanMessage);

            _mockUserIngredientRepository.Verify(r => r.DeleteRangeAsync(It.IsAny<IList<UserIngredient>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
