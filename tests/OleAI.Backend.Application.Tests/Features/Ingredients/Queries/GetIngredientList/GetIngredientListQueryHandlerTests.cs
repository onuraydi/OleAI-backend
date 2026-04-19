using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Queries.GetIngredientList;

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
    public async Task Handle_ShouldPassPaginationAndTrackingOptions_AndReturnMappedPaginate()
    {
        // Arrange
        var query = new GetIngredientListQuery { Page = 1, Size = 15 };

        var repositoryResult = new Paginate<Ingredient>
        {
            Items = new List<Ingredient> { new(Guid.NewGuid(), "Tuz", Guid.NewGuid(), MeasurementCategory.Weight) },
            PageNumber = 1,
            PageSize = 15,
            TotalItems = 40,
            TotalPages = 3
        };

        var mappedResult = new Paginate<IngredientResponse>
        {
            Items = new List<IngredientResponse> { new() { Name = "Tuz", UnitFamily = MeasurementCategory.Weight, CategoryName = "Baharat" } },
            PageNumber = 1,
            PageSize = 15,
            TotalItems = 40,
            TotalPages = 3
        };

        _mockIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        _mockMapper
            .Setup(m => m.Map<Paginate<IngredientResponse>>(repositoryResult))
            .Returns(mappedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(15);
        result.TotalItems.Should().Be(40);

        _mockIngredientRepository.Verify(r => r.GetListAsync(
            It.Is<QueryOptions<Ingredient>>(q => q.Index == 1 && q.Size == 15 && q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmpty_ShouldReturnEmptyPaginate()
    {
        // Arrange
        var repositoryResult = new Paginate<Ingredient>
        {
            Items = new List<Ingredient>(),
            PageNumber = 0,
            PageSize = 10,
            TotalItems = 0,
            TotalPages = 0
        };

        var mappedResult = new Paginate<IngredientResponse>
        {
            Items = new List<IngredientResponse>(),
            PageNumber = 0,
            PageSize = 10,
            TotalItems = 0,
            TotalPages = 0
        };

        _mockIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        _mockMapper
            .Setup(m => m.Map<Paginate<IngredientResponse>>(repositoryResult))
            .Returns(mappedResult);

        // Act
        var result = await _handler.Handle(new GetIngredientListQuery(), CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
    }
}
