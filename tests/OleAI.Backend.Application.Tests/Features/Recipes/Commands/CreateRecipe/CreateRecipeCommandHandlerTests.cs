using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IImageService> _mockImageService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IRecipeCommentRepository> _mockRecipeCommentRepository;
    private readonly Mock<ILocalLlmService> _mockLocalLlmService;
    private readonly RecipeBusinessRules _rules;
    private readonly CreateRecipeCommandHandler _handler;

    public CreateRecipeCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockImageService = new Mock<IImageService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockRecipeCommentRepository = new Mock<IRecipeCommentRepository>();
        _mockLocalLlmService = new Mock<ILocalLlmService>();

        _rules = new RecipeBusinessRules(_mockIngredientRepository.Object, _mockRecipeCommentRepository.Object);

        _handler = new CreateRecipeCommandHandler(
            _mockRecipeRepository.Object,
            _mockImageService.Object,
            _mockHttpContextAccessor.Object,
            _rules,
            _mockLocalLlmService.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCreateRecipeAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var request = new CreateRecipeRequest
        {
            Title = "Test",
            Description = "Desc",
            Visibility = RecipeVisibility.Public,
            MainImage = Mock.Of<IFormFile>()
        };

        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = request,
            Ingredients = new List<CreateRecipeIngredientRequest> { new() { IngredientId = Guid.NewGuid(), Amount = 1, UnitId = 1 } },
            Steps = new List<CreateRecipeStepRequest> { new() { StepNumber = 1, Instruction = "Step 1" } }
        };

        _mockIngredientRepository.Setup(r => r.GetCountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Ingredient, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockImageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("url", "public_id"));

        _mockLocalLlmService.Setup(s => s.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.RecipeCreatedAndPendingAdminApproval);
        _mockRecipeRepository.Verify(r => r.AddAsync(It.IsAny<Recipe>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSpamDetected_ShouldRejectRecipe()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest { Title = "Spam", Description = "Spam", Visibility = RecipeVisibility.Public },
            Ingredients = new List<CreateRecipeIngredientRequest>(),
            Steps = new List<CreateRecipeStepRequest>()
        };

        _mockLocalLlmService.Setup(s => s.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Spam detected

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.RecipeRejectedDueToSystemStandards);
        _mockRecipeRepository.Verify(r => r.AddAsync(It.Is<Recipe>(re => re.Status == RecipeStatus.Rejected), It.IsAny<CancellationToken>()), Times.Once);
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
