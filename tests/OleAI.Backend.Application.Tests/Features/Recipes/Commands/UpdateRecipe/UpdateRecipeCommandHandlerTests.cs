using System.Security.Claims;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
using OleAI.Backend.Application.Features.Recipes.Constants;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.ImageStorage;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
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
            new Mock<IIngredientRequestRepository>().Object,
            _rules,
            _mockHttpContextAccessor.Object,
            _mockLocalLlmService.Object,
            _mockImageService.Object,
            new Mock<IMediator>().Object,
            new Mock<IAiRecommendationService>().Object,
            new Mock<IServiceScopeFactory>().Object);
    }

    [Fact]
    public async Task Handle_WhenValidUpdate_ShouldUpdateRecipeAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);
        var existingRecipe = Recipe.Create(userId, "Old", "Old", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecipe);

        var command = new UpdateRecipeCommand { Request = new UpdateRecipeRequest { Id = Guid.NewGuid(), Title = "New Title", Visibility = RecipeVisibility.Private }, Ingredients = new List<CreateRecipeIngredientRequest>(), Steps = new List<UpdateRecipeStepRequest>() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(RecipeMessages.RecipeUpdatedSuccessfully);
        existingRecipe.Title.Should().Be("New Title");
    }

    [Fact]
    public async Task Handle_WhenUserNotOwner_ShouldThrowAuthorizationException()
    {
        // Arrange
        SetupUser(Guid.NewGuid());
        var ownerId = Guid.NewGuid();
        var existingRecipe = Recipe.Create(ownerId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecipe);

        var command = new UpdateRecipeCommand { Request = new UpdateRecipeRequest { Id = Guid.NewGuid() } };

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>();
    }

    [Fact]
    public async Task Handle_WhenRecipeNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        SetupUser(Guid.NewGuid());
        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        var command = new UpdateRecipeCommand { Request = new UpdateRecipeRequest { Id = Guid.NewGuid() } };

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
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

