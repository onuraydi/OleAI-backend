using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IImageService> _mockImageService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IRecipeCommentRepository> _mockRecipeCommentRepository;
    private readonly Mock<ILocalLlmService> _mockLocalLlmService;
    private readonly RecipeBusinessRules _rules;
    private readonly UpdateRecipeCommandHandler _handler;

    public UpdateRecipeCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockImageService = new Mock<IImageService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockRecipeCommentRepository = new Mock<IRecipeCommentRepository>();
        _mockLocalLlmService = new Mock<ILocalLlmService>();

        _rules = new RecipeBusinessRules(_mockIngredientRepository.Object, _mockRecipeCommentRepository.Object);

        _handler = new UpdateRecipeCommandHandler(
            _mockRecipeRepository.Object,
            _rules,
            _mockHttpContextAccessor.Object,
            _mockLocalLlmService.Object,
            _mockImageService.Object);
    }

    [Fact]
    public async Task Handle_WhenValidUpdate_ShouldUpdateRecipeAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var recipeId = Guid.NewGuid();
        var existingRecipe = Recipe.Create(userId, "Old", "Old", RecipeVisibility.Public);

        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecipe);

        _mockImageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("new_url", "new_public_id"));

        var command = new UpdateRecipeCommand
        {
            Request = new UpdateRecipeRequest
            {
                Id = recipeId,
                Title = "New Title",
                Description = "New Desc",
                Visibility = RecipeVisibility.Private
            },
            Ingredients = new List<CreateRecipeIngredientRequest>(),
            Steps = new List<UpdateRecipeStepRequest>()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.RecipeUpdatedSuccessfully);
        existingRecipe.Title.Should().Be("New Title");
        _mockRecipeRepository.Verify(r => r.UpdateAsync(existingRecipe, It.IsAny<CancellationToken>()), Times.Once);
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
