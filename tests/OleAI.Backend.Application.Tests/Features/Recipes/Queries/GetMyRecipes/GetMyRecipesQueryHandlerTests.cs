using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetMyRecipes;

public class GetMyRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly GetMyRecipesQueryHandler _handler;

    public GetMyRecipesQueryHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _handler = new GetMyRecipesQueryHandler(_mockRecipeRepository.Object, _mockMapper.Object, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnUserRecipes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var query = new GetMyRecipesQuery { Page = 0, Size = 10 };
        var recipes = new Paginate<Recipe> { Items = new List<Recipe> { Recipe.Create(userId, "My", "Desc", RecipeVisibility.Public) } };

        _mockRecipeRepository.Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<Recipe>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var response = new Paginate<RecipeListItemResponse> { Items = new List<RecipeListItemResponse> { new() { Title = "My" } } };
        _mockMapper.Setup(m => m.Map<Paginate<RecipeListItemResponse>>(recipes)).Returns(response);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        _mockRecipeRepository.Verify(r => r.GetListAsync(It.Is<QueryOptions<Recipe>>(o => o.Predicate != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupUser(Guid userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);
    }
}
