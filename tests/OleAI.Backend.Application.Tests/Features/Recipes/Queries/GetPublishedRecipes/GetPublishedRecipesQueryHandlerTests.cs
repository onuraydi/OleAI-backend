using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetPublishedRecipes;

public class GetPublishedRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetPublishedRecipesQueryHandler _handler;

    public GetPublishedRecipesQueryHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetPublishedRecipesQueryHandler(_mockRecipeRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnPublishedRecipes()
    {
        // Arrange
        var query = new GetPublishedRecipesQuery { Page = 0, Size = 10 };
        var recipes = new Paginate<Recipe> { Items = new List<Recipe> { Recipe.Create(Guid.NewGuid(), "Pub", "Desc", OleAI.Backend.Domain.Enums.RecipeVisibility.Public) } };

        _mockRecipeRepository.Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<Recipe>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var response = new Paginate<RecipeListItemResponse> { Items = new List<RecipeListItemResponse> { new() { Title = "Pub" } } };
        _mockMapper.Setup(m => m.Map<Paginate<RecipeListItemResponse>>(recipes)).Returns(response);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        _mockRecipeRepository.Verify(r => r.GetListAsync(It.Is<QueryOptions<Recipe>>(o => o.Predicate != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldFilterRecipes()
    {
        // Arrange
        var query = new GetPublishedRecipesQuery { Page = 0, Size = 10, SearchTerm = "Cake" };
        _mockRecipeRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Recipe> { Items = new List<Recipe>() });

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRecipeRepository.Verify(r => r.GetListAsync(It.Is<QueryOptions<Recipe>>(o => o.Predicate != null), It.IsAny<CancellationToken>()), Times.Once);
    }
}
