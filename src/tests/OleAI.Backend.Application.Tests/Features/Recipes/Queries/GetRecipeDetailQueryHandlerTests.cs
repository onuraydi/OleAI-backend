using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Profiles;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries;

public class GetRecipeDetailQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly Mock<IRecipeCommentRepository> _commentRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly RecipeBusinessRules _rules;
    private readonly IMapper _mapper;
    private readonly GetRecipeDetailQueryHandler _handler;

    public GetRecipeDetailQueryHandlerTests()
    {
        _rules = new RecipeBusinessRules(_ingredientRepository.Object, _commentRepository.Object);
        _mapper = CreateMapper();
        _handler = new GetRecipeDetailQueryHandler(_recipeRepository.Object, _mapper, _rules, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPublicAndPublished_ShouldReturnDetailedResponseWithoutOwnershipCheck()
    {
        var recipe = BuildRecipe(ownerId: Guid.NewGuid(), visibility: RecipeVisibility.Public, published: true, currentUserId: null);

        _recipeRepository
            .Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var response = await _handler.Handle(new GetRecipeDetailQuery { Id = recipe.Id }, CancellationToken.None);

        response.Id.Should().Be(recipe.Id);
        response.Ingredients.Should().HaveCount(1);
        response.Steps.Should().HaveCount(1);
        response.Images.Should().HaveCount(2);
        response.Statistics.TotalComments.Should().Be(2);
        response.Statistics.TotalLikes.Should().Be(1);
        response.Statistics.TotalDislikes.Should().Be(1);
        response.Statistics.TotalSaves.Should().Be(1);
        response.UserInteraction.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPrivateAndUserIsOwner_ShouldReturnDetailedResponseWithUserInteraction()
    {
        var ownerId = Guid.NewGuid();
        var recipe = BuildRecipe(ownerId: ownerId, visibility: RecipeVisibility.Private, published: true, currentUserId: ownerId);

        _recipeRepository
            .Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(ownerId));

        var response = await _handler.Handle(new GetRecipeDetailQuery { Id = recipe.Id }, CancellationToken.None);

        response.Author.Should().BeNull();
        response.UserInteraction.Should().NotBeNull();
        response.UserInteraction!.IsLiked.Should().BeTrue();
        response.UserInteraction.IsDisliked.Should().BeFalse();
        response.UserInteraction.IsSaved.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRecipeIsPrivateAndUserIsMissing_ShouldThrowUnauthorizedAccessException()
    {
        var recipe = BuildRecipe(ownerId: Guid.NewGuid(), visibility: RecipeVisibility.Private, published: true, currentUserId: null);

        _recipeRepository
            .Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                false,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        Func<Task> act = () => _handler.Handle(new GetRecipeDetailQuery { Id = recipe.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static Recipe BuildRecipe(Guid ownerId, RecipeVisibility visibility, bool published, Guid? currentUserId)
    {
        var recipe = Recipe.Create(ownerId, "Lezzetli Tarif", "Açıklama", visibility);
        if (visibility == RecipeVisibility.Public)
        {
            recipe.PassLlmFilter();
            if (published)
            {
                typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);
            }
        }
        else if (published)
        {
            typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);
        }

        recipe.AddIngredient(Guid.NewGuid(), 2, 1);
        recipe.AddStep(1, "Pişir");
        recipe.AddImage("https://cdn.example.com/main.jpg", "main-public-id", true);
        recipe.AddImage("https://cdn.example.com/step.jpg", "step-public-id", false, recipe.RecipeSteps.First().Id);

        var comments = new List<RecipeComment>
        {
            new(Guid.NewGuid(), recipe.Id, ownerId, "Harika"),
            new(Guid.NewGuid(), recipe.Id, Guid.NewGuid(), "Teşekkürler")
        };

        var likedUserId = currentUserId ?? Guid.NewGuid();
        var likedInteraction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, likedUserId);
        likedInteraction.ToggleLike();
        likedInteraction.ToggleSave();

        var dislikedInteraction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, Guid.NewGuid());
        dislikedInteraction.ToggleDislike();

        var savedInteraction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, Guid.NewGuid());
        savedInteraction.ToggleSave();

        typeof(Recipe).GetProperty(nameof(Recipe.Comments))!.SetValue(recipe, comments);
        typeof(Recipe).GetProperty(nameof(Recipe.Interactions))!.SetValue(recipe, new List<RecipeInteraction> { likedInteraction, dislikedInteraction, savedInteraction });

        return recipe;
    }
}
