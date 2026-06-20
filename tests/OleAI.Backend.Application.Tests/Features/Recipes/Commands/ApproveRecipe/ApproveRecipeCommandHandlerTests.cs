using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly Mock<IAiRecommendationService> _mockAiRecommendationService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ApproveRecipeCommandHandler _handler;

    public ApproveRecipeCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        var mockCommentRepo = new Mock<IRecipeCommentRepository>();
        _recipeBusinessRules = new RecipeBusinessRules(_mockIngredientRepository.Object, mockCommentRepo.Object);
        _mockAiRecommendationService = new Mock<IAiRecommendationService>();
        _mockCacheService = new Mock<ICacheService>();

        _handler = new ApproveRecipeCommandHandler(
            _mockRecipeRepository.Object,
            _mockIngredientRepository.Object,
            _mockIngredientRequestRepository.Object,
            _recipeBusinessRules,
            _mockAiRecommendationService.Object,
            _mockCacheService.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldApproveRecipeAndHandlePendingIngredients()
    {
        var recipeId = Guid.NewGuid();
        var command = new ApproveRecipeCommand
        {
            Id = recipeId,
            EditedDescription = "New Desc",
            EditedSteps = new List<EditedStepDto> { new EditedStepDto { StepNumber = 1, Instruction = "New Step" } }
        };

        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeDifficulty.Easy, 1, 10, 10, RecipeVisibility.Public);
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "PendingIng", Guid.NewGuid());
        recipe.AddIngredient(null, ingredientRequest.Id, 1, 0); // Pending ingredient
        recipe.AddStep(1, "Old Step");

        _mockRecipeRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(),
            It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var requestPaginated = new Paginate<IngredientRequest> { Items = new List<IngredientRequest> { ingredientRequest } };
        _mockIngredientRequestRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<IngredientRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestPaginated);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Tarif başarıyla onaylandı ve topluluğa açıldı.", result);
        Assert.Equal(RecipeStatus.Published, recipe.Status);
        Assert.Equal("New Desc", recipe.Description);

        _mockIngredientRepository.Verify(r => r.AddRangeAsync(It.IsAny<List<Ingredient>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockIngredientRequestRepository.Verify(r => r.UpdateRangeAsync(It.IsAny<List<IngredientRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockAiRecommendationService.Verify(a => a.SyncRecipeAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<OleAI.Backend.Application.Services.AiRecommendation.RecipeIngredientAiDto>>(), It.IsAny<string>(), null, true, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithLlmReviewJson_ShouldParseAndApplyCorrections()
    {
        var recipeId = Guid.NewGuid();
        var command = new ApproveRecipeCommand { Id = recipeId };

        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeDifficulty.Easy, 1, 10, 10, RecipeVisibility.Public);
        recipe.AddStep(1, "Old Step 1");
        recipe.SetLlmReviewResult("{\"isApproved\": true, \"correctedDescription\": \"Llm Desc\", \"correctedSteps\": [\"Llm Step 1\"]}");

        _mockRecipeRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(),
            It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Tarif başarıyla onaylandı ve topluluğa açıldı.", result);
        Assert.Equal("Llm Desc", recipe.Description);
        Assert.Equal("Llm Step 1", recipe.RecipeSteps.First().Instruction);
    }
}
