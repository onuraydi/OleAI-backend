using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Queries.GetMyPantry;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Queries
{
    public class GetMyPantryQueryHandlerTests
    {
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetMyPantryQueryHandler _handler;

        public GetMyPantryQueryHandlerTests()
        {
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _mockMapper = new Mock<IMapper>();

            _handler = new GetMyPantryQueryHandler(_mockUserIngredientRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfPantryItemResponses()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetMyPantryQuery { UserId = userId };

            var pantryItems = new List<UserIngredient>
            {
                UserIngredient.CreateStandard(Guid.NewGuid(), userId, Guid.NewGuid(), 2, null),
                UserIngredient.CreateStandard(Guid.NewGuid(), userId, Guid.NewGuid(), 5, null)
            };

            var paginatedItems = new Paginate<UserIngredient> { Items = pantryItems };

            var mappedResponse = new List<PantryItemResponse>
            {
                new PantryItemResponse { Id = pantryItems[0].Id, Quantity = 2 },
                new PantryItemResponse { Id = pantryItems[1].Id, Quantity = 5 }
            };

            _mockUserIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedItems);

            _mockMapper
                .Setup(m => m.Map<List<PantryItemResponse>>(pantryItems))
                .Returns(mappedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Quantity.Should().Be(2);

            _mockUserIngredientRepository.Verify(r => r.GetListAsync(
                It.Is<QueryOptions<UserIngredient>>(opt => opt.EnableTracking == false),
                It.IsAny<CancellationToken>()), Times.Once);

            _mockMapper.Verify(m => m.Map<List<PantryItemResponse>>(pantryItems), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoItemsExist_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetMyPantryQuery { UserId = Guid.NewGuid() };
            var emptyItems = new List<UserIngredient>();
            var paginatedItems = new Paginate<UserIngredient> { Items = emptyItems };

            _mockUserIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedItems);

            _mockMapper
                .Setup(m => m.Map<List<PantryItemResponse>>(emptyItems))
                .Returns(new List<PantryItemResponse>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
