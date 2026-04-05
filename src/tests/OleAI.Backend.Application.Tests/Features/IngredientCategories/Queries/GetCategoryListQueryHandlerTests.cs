using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Queries
{
    public class GetCategoryListQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetCategoryListQueryHandler _handler;

        public GetCategoryListQueryHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetCategoryListQueryHandler(_mockCategoryRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfCategoryResponses()
        {
            // Arrange
            var query = new GetCategoryListQuery();

            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Vegetables" },
                new Category { Id = Guid.NewGuid(), Name = "Fruits" }
            };

            var paginatedCategories = new Paginate<Category>
            {
                Items = categories
            };

            var mappedResponse = new List<CategoryResponse>
            {
                new CategoryResponse { Id = categories[0].Id, Name = "Vegetables" },
                new CategoryResponse { Id = categories[1].Id, Name = "Fruits" }
            };

            _mockCategoryRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedCategories);

            _mockMapper
                .Setup(m => m.Map<List<CategoryResponse>>(categories))
                .Returns(mappedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Vegetables");
            result[1].Name.Should().Be("Fruits");

            _mockCategoryRepository.Verify(r => r.GetListAsync(It.Is<QueryOptions<Category>>(opt => opt.Size == 100 && opt.EnableTracking == false), It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(m => m.Map<List<CategoryResponse>>(categories), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoCategoriesExist_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetCategoryListQuery();
            var emptyCategories = new List<Category>();
            var paginatedCategories = new Paginate<Category> { Items = emptyCategories };
            var emptyResponseList = new List<CategoryResponse>();

            _mockCategoryRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedCategories);

            _mockMapper
                .Setup(m => m.Map<List<CategoryResponse>>(emptyCategories))
                .Returns(emptyResponseList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
