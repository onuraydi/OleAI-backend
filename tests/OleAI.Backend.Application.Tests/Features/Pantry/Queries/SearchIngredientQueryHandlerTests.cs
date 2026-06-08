using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Queries.SearchIngredient;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Queries;

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
    public async Task Handle_WhenKeywordTooShort_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new SearchIngredientQuery { Keyword = "a" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _mockCacheService.Verify(c => c.GetAsync<List<IngredientSearchResultResponse>>(It.IsAny<string>()), Times.Never);
        _mockIngredientRepository.Verify(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ShouldUseCacheAndApplyRanking()
    {
        // Arrange
        var cachedCatalog = new List<IngredientSearchResultResponse>
        {
            new() { MasterIngredientId = Guid.NewGuid(), DisplayName = "Brown sugar", UnitFamily = MeasurementCategory.Weight },
            new() { MasterIngredientId = Guid.NewGuid(), DisplayName = "Sugar", UnitFamily = MeasurementCategory.Weight },
            new() { MasterIngredientId = Guid.NewGuid(), DisplayName = "Super sweet", UnitFamily = MeasurementCategory.Weight }
        };

        _mockCacheService
            .Setup(c => c.GetAsync<List<IngredientSearchResultResponse>>(It.IsAny<string>()))
            .ReturnsAsync(cachedCatalog);

        var query = new SearchIngredientQuery { Keyword = "su" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].DisplayName.Should().Be("Sugar");
        result[1].DisplayName.Should().Be("Super sweet");
        result[2].DisplayName.Should().Be("Brown sugar");

        _mockIngredientRepository.Verify(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockCacheService.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<List<IngredientSearchResultResponse>>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ShouldLoadFromDatabaseAndCacheCatalog()
    {
        // Arrange
        var ingredient = new Ingredient(Guid.NewGuid(), "Tomato", Guid.NewGuid(), MeasurementCategory.Count);
        ingredient.Aliases.Add(new IngredientAlias(Guid.NewGuid(), ingredient.Id, "Domates"));

        _mockCacheService
            .Setup(c => c.GetAsync<List<IngredientSearchResultResponse>>(It.IsAny<string>()))
            .ReturnsAsync((List<IngredientSearchResultResponse>?)null);

        _mockIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Ingredient> { Items = new List<Ingredient> { ingredient } });

        var query = new SearchIngredientQuery { Keyword = "to" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain(r => r.DisplayName == "Tomato" && r.IsAlias == false);

        _mockIngredientRepository.Verify(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(c => c.SetAsync<List<IngredientSearchResultResponse>>(
            "Pantry:AllIngredientsCatalog:v2",
            It.Is<List<IngredientSearchResultResponse>>(list => list.Count >= 2),
            It.Is<TimeSpan?>(t => t == TimeSpan.FromHours(6)),
            null), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnMaximum20Results()
    {
        // Arrange
        var catalog = Enumerable.Range(1, 30)
            .Select(i => new IngredientSearchResultResponse
            {
                MasterIngredientId = Guid.NewGuid(),
                DisplayName = $"su-item-{i}",
                UnitFamily = MeasurementCategory.Count
            }).ToList();

        _mockCacheService
            .Setup(c => c.GetAsync<List<IngredientSearchResultResponse>>(It.IsAny<string>()))
            .ReturnsAsync(catalog);

        // Act
        var result = await _handler.Handle(new SearchIngredientQuery { Keyword = "su" }, CancellationToken.None);

        // Assert
        result.Should().HaveCount(20);
    }

    [Fact]
    public async Task Handle_ShouldSearchCaseInsensitive()
    {
        // Arrange
        var cachedCatalog = new List<IngredientSearchResultResponse>
        {
            new() { MasterIngredientId = Guid.NewGuid(), DisplayName = "SUGAR", UnitFamily = MeasurementCategory.Weight },
            new() { MasterIngredientId = Guid.NewGuid(), DisplayName = "Salt", UnitFamily = MeasurementCategory.Weight }
        };

        _mockCacheService
            .Setup(c => c.GetAsync<List<IngredientSearchResultResponse>>(It.IsAny<string>()))
            .ReturnsAsync(cachedCatalog);

        // Act
        var result = await _handler.Handle(new SearchIngredientQuery { Keyword = "sug" }, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result[0].DisplayName.Should().Be("SUGAR");
    }
}
