using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.ChangeRecipeVisibility;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.ChangeRecipeVisibility;

public class ChangeRecipeVisibilityCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ILocalLlmService> _mockLlmService;
    private readonly RecipeBusinessRules _rules;
    private readonly ChangeRecipeVisibilityCommandHandler _handler;

    public ChangeRecipeVisibilityCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockLlmService = new Mock<ILocalLlmService>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), Mock.Of<IRecipeCommentRepository>());
        _handler = new ChangeRecipeVisibilityCommandHandler(_mockRecipeRepository.Object, _rules, _mockHttpContextAccessor.Object, _mockLlmService.Object);
    }

    [Fact]
    public async Task Handle_WhenOwnerChangesToPrivate_ShouldUpdateAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var recipe = Recipe.Create(userId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);
        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var command = new ChangeRecipeVisibilityCommand { Id = recipe.Id, Visibility = RecipeVisibility.Private };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();
        recipe.Visibility.Should().Be(RecipeVisibility.Private);
        _mockRecipeRepository.Verify(r => r.UpdateAsync(recipe, It.IsAny<CancellationToken>()), Times.Once);
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

