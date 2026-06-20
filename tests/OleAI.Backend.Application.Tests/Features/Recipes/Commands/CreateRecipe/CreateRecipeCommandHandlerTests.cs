using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly Mock<IImageService> _mockImageService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly Mock<ILocalLlmService> _mockLocalLlmService;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<IAiRecommendationService> _mockAiRecommendationService;
    private readonly CreateRecipeCommandHandler _handler;

    public CreateRecipeCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _mockImageService = new Mock<IImageService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockLocalLlmService = new Mock<ILocalLlmService>();
        _mockMediator = new Mock<IMediator>();
        _mockAiRecommendationService = new Mock<IAiRecommendationService>();

        var mockIngredientRepo = new Mock<IIngredientRepository>();
        var mockCommentRepo = new Mock<IRecipeCommentRepository>();
        _recipeBusinessRules = new RecipeBusinessRules(mockIngredientRepo.Object, mockCommentRepo.Object);

        _handler = new CreateRecipeCommandHandler(
            _mockRecipeRepository.Object,
            _mockIngredientRequestRepository.Object,
            _mockImageService.Object,
            _mockHttpContextAccessor.Object,
            _recipeBusinessRules, // Needs full setup if used, but CheckIfIngredientsExistAsync is bypassed if we mock appropriately, wait CheckIfIngredientsExistAsync uses IngredientRepository. We should mock that!
            _mockLocalLlmService.Object,
            _mockMediator.Object,
            _mockAiRecommendationService.Object
        );
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest { Title = "Test" },
            Ingredients = new List<CreateRecipeIngredientRequest>(),
            Steps = new List<CreateRecipeStepRequest>()
        };

        var mockIngredientRepo = new Mock<IIngredientRepository>();
        var handlerWithMockedRules = new CreateRecipeCommandHandler(
            _mockRecipeRepository.Object,
            _mockIngredientRequestRepository.Object,
            _mockImageService.Object,
            _mockHttpContextAccessor.Object,
            new RecipeBusinessRules(mockIngredientRepo.Object, new Mock<IRecipeCommentRepository>().Object),
            _mockLocalLlmService.Object,
            _mockMediator.Object,
            _mockAiRecommendationService.Object
        );

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handlerWithMockedRules.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCreateRecipeAndReturnSuccessMessage()
    {
        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var mockIngredientRepo = new Mock<IIngredientRepository>();
        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest
            {
                Title = "Test Recipe",
                Description = "Test Desc",
                Difficulty = OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy,
                Servings = 2,
                PrepTime = 10,
                CookTime = 20,
                Visibility = OleAI.Backend.Domain.Enums.RecipeVisibility.Public,
                CategoryIds = new List<int> { 1 },
                TagIds = new List<int> { 1 },
                MainImage = null
            },
            Ingredients = new List<CreateRecipeIngredientRequest>
            {
                new CreateRecipeIngredientRequest { IngredientId = Guid.NewGuid(), Amount = 1, UnitId = 0 },
                new CreateRecipeIngredientRequest { RequestedName = "New Ing", Amount = 1, UnitId = 0 }
            },
            Steps = new List<CreateRecipeStepRequest>
            {
                new CreateRecipeStepRequest { StepNumber = 1, Instruction = "Step 1" }
            }
        };

        var handler = new CreateRecipeCommandHandler(
            _mockRecipeRepository.Object,
            _mockIngredientRequestRepository.Object,
            _mockImageService.Object,
            _mockHttpContextAccessor.Object,
            new RecipeBusinessRules(mockIngredientRepo.Object, new Mock<IRecipeCommentRepository>().Object),
            _mockLocalLlmService.Object,
            _mockMediator.Object,
            _mockAiRecommendationService.Object
        );

        _mockImageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("http://test", "123"));

        mockIngredientRepo.Setup(r => r.GetCountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Ingredient, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockLocalLlmService.Setup(s => s.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(OleAI.Backend.Application.Features.Recipes.Constants.RecipeMessages.RecipeCreatedAndPendingAdminApproval);
        _mockRecipeRepository.Verify(r => r.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockIngredientRequestRepository.Verify(r => r.AddRangeAsync(It.IsAny<List<IngredientRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
