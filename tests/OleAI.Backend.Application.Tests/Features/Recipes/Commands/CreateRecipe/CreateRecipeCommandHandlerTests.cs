using System.Security.Claims;
using FluentAssertions;
using MediatR;
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
            new Mock<IIngredientRequestRepository>().Object,
            _mockImageService.Object,
            _mockHttpContextAccessor.Object,
            _rules,
            _mockLocalLlmService.Object,
            new Mock<IMediator>().Object);
    }

    [Fact]
    public async Task Handle_WhenValidPublicRequest_ShouldCallLlmAndReturnPendingApproval()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest { Title = "T", Visibility = RecipeVisibility.Public, MainImage = Mock.Of<IFormFile>() },
            Ingredients = new List<CreateRecipeIngredientRequest>(),
            Steps = new List<CreateRecipeStepRequest>()
        };

        _mockImageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("url", "id"));
        _mockLocalLlmService.Setup(s => s.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeCreatedAndPendingAdminApproval);
        _mockLocalLlmService.Verify(s => s.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPrivateRequest_ShouldNotCallLlmAndReturnSavedAsPrivate()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest { Title = "T", Visibility = RecipeVisibility.Private, MainImage = Mock.Of<IFormFile>() },
            Ingredients = new List<CreateRecipeIngredientRequest>(),
            Steps = new List<CreateRecipeStepRequest>()
        };

        _mockImageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("url", "id"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(RecipeMessages.RecipeCreatedAndSavedAsPrivate);
        _mockLocalLlmService.Verify(s => s.CheckRecipeContentForSpamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithStepImages_ShouldUploadStepImages()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var command = new CreateRecipeCommand
        {
            CreateRecipeRequest = new CreateRecipeRequest { Title = "T", Visibility = RecipeVisibility.Private, MainImage = Mock.Of<IFormFile>() },
            Ingredients = new List<CreateRecipeIngredientRequest>(),
            Steps = new List<CreateRecipeStepRequest>
            {
                new() { StepNumber = 1, Instruction = "I", StepImage = Mock.Of<IFormFile>() }
            }
        };

        _mockImageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("url", "id"));

        await _handler.Handle(command, CancellationToken.None);

        _mockImageService.Verify(s => s.UploadAsync(It.IsAny<IFormFile>(), "recipes/steps", It.IsAny<CancellationToken>()), Times.Once);
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
