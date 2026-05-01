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
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), Mock.Of<IRecipeCommentRepository>());

        _handler = new GetRecipeDetailQueryHandler(
            _mockRecipeRepository.Object,
            _mockMapper.Object,
            _rules,
            _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPublic_ShouldReturnDetail()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        recipe.ApproveByAdmin(); // Status: Published

        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var response = new RecipeDetailResponse { Id = recipeId, Title = "Title" };
        _mockMapper.Setup(m => m.Map<RecipeDetailResponse>(recipe)).Returns(response);

        var query = new GetRecipeDetailQuery { Id = recipeId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Title");
        result.Statistics.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPrivateAndNotOwner_ShouldThrowAuthorizationException()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Private", "Desc", RecipeVisibility.Private);
        SetupUser(Guid.NewGuid()); // Other user

        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var query = new GetRecipeDetailQuery { Id = recipe.Id };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>();
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
