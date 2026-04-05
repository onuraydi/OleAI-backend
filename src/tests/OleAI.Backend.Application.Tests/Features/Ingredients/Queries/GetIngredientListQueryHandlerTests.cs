using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Queries
{
    public class GetIngredientListQueryHandlerTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetIngredientListQueryHandler _handler;

        public GetIngredientListQueryHandlerTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetIngredientListQueryHandler(_mockIngredientRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedIngredientResponses()
        {
            // Arrange
            var query = new GetIngredientListQuery { Page = 0, Size = 10 };

            var ingredients = new List<Ingredient>
            {
                new Ingredient { Id = Guid.NewGuid(), Name = "Vegetable" },
                new Ingredient { Id = Guid.NewGuid(), Name = "Fruit" }
            };

            var paginatedIngredients = new Paginate<Ingredient> { Items = ingredients };

            var mappedResponse = new Paginate<IngredientResponse>
            {
                Items = new List<IngredientResponse>
                {
                    new IngredientResponse { Id = ingredients[0].Id, Name = "Vegetable" },
                    new IngredientResponse { Id = ingredients[1].Id, Name = "Fruit" }
                }
            };

            _mockIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedIngredients);

            _mockMapper
                .Setup(m => m.Map<Paginate<IngredientResponse>>(paginatedIngredients))
                .Returns(mappedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items[0].Name.Should().Be("Vegetable");
            result.Items[1].Name.Should().Be("Fruit");

            _mockIngredientRepository.Verify(r => r.GetListAsync(
                It.Is<QueryOptions<Ingredient>>(opt => opt.Index == 0 && opt.Size == 10 && opt.EnableTracking == false), 
                It.IsAny<CancellationToken>()), Times.Once);
            
            _mockMapper.Verify(m => m.Map<Paginate<IngredientResponse>>(paginatedIngredients), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoIngredientsExist_ShouldReturnEmptyPaginatedList()
        {
            // Arrange
            var query = new GetIngredientListQuery { Page = 0, Size = 10 };
            
            var paginatedIngredients = new Paginate<Ingredient> { Items = new List<Ingredient>() };
            var mappedResponse = new Paginate<IngredientResponse> { Items = new List<IngredientResponse>() };

            _mockIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedIngredients);

            _mockMapper
                .Setup(m => m.Map<Paginate<IngredientResponse>>(paginatedIngredients))
                .Returns(mappedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }
    }
}
