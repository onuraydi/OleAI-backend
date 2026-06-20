using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetCategories;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Queries.GetCategories;

public class GetRecipeCategoriesQueryHandlerTests
{
    private readonly Mock<IRecipeCategoryRepository> _recipeCategoryRepositoryMock;
    private readonly GetRecipeCategoriesQueryHandler _handler;

    public GetRecipeCategoriesQueryHandlerTests()
    {
        _recipeCategoryRepositoryMock = new Mock<IRecipeCategoryRepository>();

        _handler = new GetRecipeCategoriesQueryHandler(_recipeCategoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnRecipeCategoriesList()
    {
        // Arrange
        var query = new GetRecipeCategoriesQuery();

        var categories = new List<OleAI.Backend.Domain.Entities.RecipeCategory>
        {
            new OleAI.Backend.Domain.Entities.RecipeCategory(1, "Desserts", "icon-dessert"),
            new OleAI.Backend.Domain.Entities.RecipeCategory(2, "Appetizers", "icon-appetizer")
        };

        var paginatedCategories = new Paginate<OleAI.Backend.Domain.Entities.RecipeCategory>
        {
            Items = categories,
            PageNumber = 1,
            PageSize = 1000,
            TotalItems = 2,
            TotalPages = 1
        };

        _recipeCategoryRepositoryMock.Setup(x => x.GetListAsync(
                It.IsAny<QueryOptions<OleAI.Backend.Domain.Entities.RecipeCategory>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedCategories);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        // Results are sorted by Name (Appetizers then Desserts)
        result[0].Name.Should().Be("Appetizers");
        result[1].Name.Should().Be("Desserts");

        _recipeCategoryRepositoryMock.Verify(x => x.GetListAsync(
            It.Is<QueryOptions<OleAI.Backend.Domain.Entities.RecipeCategory>>(q => q.Size == 1000 && q.EnableTracking == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
