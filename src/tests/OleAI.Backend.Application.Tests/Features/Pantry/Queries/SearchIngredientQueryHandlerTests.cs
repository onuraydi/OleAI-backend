using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Queries.SearchIngredient;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Queries
{
    public class SearchIngredientQueryHandlerTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly SearchIngredientQueryHandler _handler;

        public SearchIngredientQueryHandlerTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockCacheService = new Mock<ICacheService>();

            _handler = new SearchIngredientQueryHandler(_mockIngredientRepository.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task Handle_WhenKeywordIsLessThanMinimumLength_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new SearchIngredientQuery { Keyword = "A" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            _mockCacheService.Verify(c => c.GetAsync<List<IngredientSearchResultResponse>>(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenKeywordIsNull_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new SearchIngredientQuery { Keyword = null! };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            _mockCacheService.Verify(c => c.GetAsync<List<IngredientSearchResultResponse>>(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenItemsCached_ShouldFilterAndReturnTop20OrderedResults()
        {
            // Arrange
            var query = new SearchIngredientQuery { Keyword = "apple" };

            var cachedCatalog = new List<IngredientSearchResultResponse>
            {
                new IngredientSearchResultResponse { Id = Guid.NewGuid(), DisplayName = "Green Apple", MasterIngredientId = Guid.NewGuid() },
                new IngredientSearchResultResponse { Id = Guid.NewGuid(), DisplayName = "Apple", MasterIngredientId = Guid.NewGuid() },
                new IngredientSearchResultResponse { Id = Guid.NewGuid(), DisplayName = "Pineapple", MasterIngredientId = Guid.NewGuid() },
                new IngredientSearchResultResponse { Id = Guid.NewGuid(), DisplayName = "Apple Pie", MasterIngredientId = Guid.NewGuid() },
                new IngredientSearchResultResponse { Id = Guid.NewGuid(), DisplayName = "Banana", MasterIngredientId = Guid.NewGuid() }
            };

            _mockCacheService
                .Setup(c => c.GetAsync<List<IngredientSearchResultResponse>>("Pantry:AllIngredientsCatalog"))
                .ReturnsAsync(cachedCatalog);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(4); // Banana does not contain "apple"
            
            // StartsWith ("Apple") should come first, then alphabetical
            result[0].DisplayName.Should().Be("Apple");
            result[1].DisplayName.Should().Be("Apple Pie");
            result[2].DisplayName.Should().Be("Green Apple");
            result[3].DisplayName.Should().Be("Pineapple");

            _mockIngredientRepository.Verify(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCatalogNotCached_ShouldLoadFromDatabaseAndCache()
        {
            // Arrange
            var query = new SearchIngredientQuery { Keyword = "meat" };

            _mockCacheService
                .Setup(c => c.GetAsync<List<IngredientSearchResultResponse>>("Pantry:AllIngredientsCatalog"))
                .ReturnsAsync((List<IngredientSearchResultResponse>)null!);

            var dbIngredients = new List<Ingredient>
            {
                new Ingredient { Id = Guid.NewGuid(), Name = "Meat", Aliases = new List<IngredientAlias>() }
            };

            var paginatedIngredients = new Paginate<Ingredient> { Items = dbIngredients };

            _mockIngredientRepository
                .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedIngredients);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].DisplayName.Should().Be("Meat");

            _mockIngredientRepository.Verify(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockCacheService.Verify(c => c.SetAsync("Pantry:AllIngredientsCatalog", It.IsAny<List<IngredientSearchResultResponse>>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
