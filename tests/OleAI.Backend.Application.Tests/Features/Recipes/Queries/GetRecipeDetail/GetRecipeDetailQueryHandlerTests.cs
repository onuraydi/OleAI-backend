using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetRecipeDetail;

public class GetRecipeDetailQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly GetRecipeDetailQueryHandler _handler;

    public GetRecipeDetailQueryHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var recipeCommentRepositoryMock = new Mock<IRecipeCommentRepository>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), recipeCommentRepositoryMock.Object);

        _handler = new GetRecipeDetailQueryHandler(
            _mockRecipeRepository.Object,
            recipeCommentRepositoryMock.Object,
            _mockMapper.Object,
            _rules,
            _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPublicAndPublished_ShouldReturnDetail()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);
        recipe.PassLlmFilter();
        recipe.ApproveByAdmin();

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        SetInteractions(recipe, new List<RecipeInteraction>());
        SetComments(recipe, new List<RecipeComment>());

        _mockMapper.Setup(m => m.Map<RecipeDetailResponse>(recipe)).Returns(new RecipeDetailResponse());

        var result = await _handler.Handle(new GetRecipeDetailQuery { Id = Guid.NewGuid() }, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPrivateButOwnerIsQuerying_ShouldReturnDetail()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);
        var recipe = Recipe.Create(userId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Private);

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        SetInteractions(recipe, new List<RecipeInteraction>());
        SetComments(recipe, new List<RecipeComment>());

        _mockMapper.Setup(m => m.Map<RecipeDetailResponse>(recipe)).Returns(new RecipeDetailResponse());

        var result = await _handler.Handle(new GetRecipeDetailQuery { Id = recipe.Id }, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenUserLoggedIn_ShouldReturnUserInteraction()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);
        recipe.PassLlmFilter();
        recipe.ApproveByAdmin();

        var interaction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, userId);
        interaction.ToggleLike();
        SetInteractions(recipe, new List<RecipeInteraction> { interaction });
        SetComments(recipe, new List<RecipeComment>());

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _mockMapper.Setup(m => m.Map<RecipeDetailResponse>(recipe)).Returns(new RecipeDetailResponse());

        var result = await _handler.Handle(new GetRecipeDetailQuery { Id = recipe.Id }, CancellationToken.None);

        result.UserInteraction.Should().NotBeNull();
        result.UserInteraction!.IsLiked.Should().BeTrue();
    }

    private void SetupUser(Guid userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);
    }

    private void SetInteractions(Recipe recipe, List<RecipeInteraction> interactions)
    {
        var property = typeof(Recipe).GetProperty(nameof(Recipe.Interactions));
        property?.SetValue(recipe, interactions);
    }

    private void SetComments(Recipe recipe, List<RecipeComment> comments)
    {
        var property = typeof(Recipe).GetProperty(nameof(Recipe.Comments));
        property?.SetValue(recipe, comments);
    }
}

