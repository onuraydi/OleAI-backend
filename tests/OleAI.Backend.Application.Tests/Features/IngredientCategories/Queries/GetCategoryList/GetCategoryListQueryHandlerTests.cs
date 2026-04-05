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

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Queries.GetCategoryList;

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
    public async Task Handle_ShouldRequestUntrackedFirst100Items_AndMapResult()
    {
        // Arrange
        var categories = new List<Category>
        {
            new(Guid.NewGuid(), "Bakliyat"),
            new(Guid.NewGuid(), "Baharat")
        };

        var mapped = new List<CategoryResponse>
        {
            new() { Id = categories[0].Id, Name = categories[0].Name },
            new() { Id = categories[1].Id, Name = categories[1].Name }
        };

        _mockCategoryRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Category>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Category> { Items = categories });

        _mockMapper
            .Setup(m => m.Map<List<CategoryResponse>>(categories))
            .Returns(mapped);

        // Act
        var result = await _handler.Handle(new GetCategoryListQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(mapped);

        _mockCategoryRepository.Verify(r => r.GetListAsync(
            It.Is<QueryOptions<Category>>(q => q.Size == 100 && q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockMapper.Verify(m => m.Map<List<CategoryResponse>>(categories), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoCategoryExists_ShouldReturnEmptyList()
    {
        // Arrange
        var empty = new List<Category>();

        _mockCategoryRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Category>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Category> { Items = empty });

        _mockMapper
            .Setup(m => m.Map<List<CategoryResponse>>(empty))
            .Returns(new List<CategoryResponse>());

        // Act
        var result = await _handler.Handle(new GetCategoryListQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
