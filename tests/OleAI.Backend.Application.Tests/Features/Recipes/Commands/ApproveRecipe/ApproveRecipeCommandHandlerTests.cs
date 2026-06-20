using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldApproveRecipe()
    {
        var mockRepo = new Mock<IRecipeRepository>();
        var mockIngRepo = new Mock<IIngredientRepository>();
        var mockIngReqRepo = new Mock<IIngredientRequestRepository>();
        var mockCommentRepo = new Mock<IRecipeCommentRepository>();
        var rules = new RecipeBusinessRules(mockIngRepo.Object, mockCommentRepo.Object);
        var mockAi = new Mock<IAiRecommendationService>();
        var mockCache = new Mock<ICacheService>();

        var handler = new ApproveRecipeCommandHandler(mockRepo.Object, mockIngRepo.Object, mockIngReqRepo.Object, rules, mockAi.Object, mockCache.Object);

        var recipeId = Guid.NewGuid();
        var recipe = Recipe.Create(Guid.NewGuid(), "T", "D", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);
        var idProp = typeof(Entity<Guid>).GetProperty("Id");
        idProp.SetValue(recipe, recipeId);

        var step = new RecipeStep { Id = Guid.NewGuid(), RecipeId = recipeId, StepNumber = 1, Instruction = "Old" };
        var stepsField = typeof(Recipe).GetField("_recipeSteps", BindingFlags.NonPublic | BindingFlags.Instance);
        var stepsList = (List<RecipeStep>)stepsField.GetValue(recipe);
        stepsList.Add(step);

        mockRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), false, true, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);

        var request = new ApproveRecipeCommand
        {
            Id = recipeId,
            EditedDescription = "New Desc",
            EditedSteps = new List<EditedStepDto> { new EditedStepDto { StepNumber = 1, Instruction = "New Inst" } }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().Be("Tarif başarıyla onaylandı ve topluluğa açıldı.");
        recipe.Status.Should().Be(OleAI.Backend.Domain.Enums.RecipeStatus.Published);
        recipe.Description.Should().Be("New Desc");
        recipe.RecipeSteps.First().Instruction.Should().Be("New Inst");

        mockRepo.Verify(r => r.UpdateAsync(recipe, It.IsAny<CancellationToken>()), Times.Once);
    }
}
