using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.ToggleInteraction;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.ToggleInteraction;

public class ToggleInteractionCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IRecipeInteractionRepository> _mockInteractionRepository;
    private readonly Mock<IUserProfileVectorService> _mockVectorService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly ToggleInteractionCommandHandler _handler;

    public ToggleInteractionCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockInteractionRepository = new Mock<IRecipeInteractionRepository>();
        _mockVectorService = new Mock<IUserProfileVectorService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), Mock.Of<IRecipeCommentRepository>());

        _handler = new ToggleInteractionCommandHandler(
            _mockRecipeRepository.Object,
            _mockInteractionRepository.Object,
            _rules,
            _mockVectorService.Object,
            _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenLike_ShouldToggleLikeAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var recipe = Recipe.Create(userId, "Title", "Desc", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        recipe.ApproveByAdmin();

        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _mockInteractionRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<RecipeInteraction, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<RecipeInteraction>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeInteraction, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RecipeInteraction?)null);

        var command = new ToggleInteractionCommand { RecipeId = recipe.Id, InteractionType = "Like" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Like işlemi başarıyla gerçekleştirildi.");
        _mockInteractionRepository.Verify(r => r.AddAsync(It.IsAny<RecipeInteraction>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockInteractionRepository.Verify(r => r.UpdateAsync(It.Is<RecipeInteraction>(i => i.IsLiked), It.IsAny<CancellationToken>()), Times.Once);
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
