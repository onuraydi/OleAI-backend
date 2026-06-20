using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPendingAdminRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetPendingAdminRecipes;

public class GetPendingAdminRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetPendingAdminRecipesQueryHandler _handler;

    public GetPendingAdminRecipesQueryHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetPendingAdminRecipesQueryHandler(_mockRecipeRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnPendingRecipes()
    {
        // Arrange
        var query = new GetPendingAdminRecipesQuery { Page = 0, Size = 10 };
        var recipes = new Paginate<Recipe> { Items = new List<Recipe> { Recipe.Create(Guid.NewGuid(), "Pending", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public) } };

        _mockRecipeRepository.Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<Recipe>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var response = new Paginate<PendingAdminRecipeListResponse> { Items = new List<PendingAdminRecipeListResponse> { new() { Title = "Pending" } } };
        _mockMapper.Setup(m => m.Map<Paginate<PendingAdminRecipeListResponse>>(recipes)).Returns(response);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        _mockRecipeRepository.Verify(r => r.GetListAsync(It.Is<QueryOptions<Recipe>>(o => o.Predicate != null), It.IsAny<CancellationToken>()), Times.Once);
    }
}

