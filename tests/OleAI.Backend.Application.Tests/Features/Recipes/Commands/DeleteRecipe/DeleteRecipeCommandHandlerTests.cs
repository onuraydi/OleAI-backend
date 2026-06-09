using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IImageService> _mockImageService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly DeleteRecipeCommandHandler _handler;

    public DeleteRecipeCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockImageService = new Mock<IImageService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), Mock.Of<IRecipeCommentRepository>());
        _handler = new DeleteRecipeCommandHandler(_mockRecipeRepository.Object, _rules, _mockImageService.Object, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldDeleteRecipe()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);
        var recipe = Recipe.Create(userId, "Title", "Desc", RecipeVisibility.Public);
        recipe.AddImage("url", "p1", true);

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var command = new DeleteRecipeCommand { Id = recipe.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.RecipeDeletedWithRelatedContent);
        _mockImageService.Verify(s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRecipeRepository.Verify(r => r.DeleteAsync(recipe, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotOwner_ShouldThrowAuthorizationException()
    {
        // Arrange
        SetupUser(Guid.NewGuid());
        var ownerId = Guid.NewGuid();
        var recipe = Recipe.Create(ownerId, "Title", "Desc", RecipeVisibility.Public);

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var command = new DeleteRecipeCommand { Id = recipe.Id };

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

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
