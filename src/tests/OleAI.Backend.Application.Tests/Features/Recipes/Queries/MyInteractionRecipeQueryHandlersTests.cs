using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Profiles;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMySavedRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Tests.TestInfrastructure;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries;

public class GetMyLikedRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeInteractionRepository> _interactionRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly IMapper _mapper;
    private readonly GetMyLikedRecipesQueryHandler _handler;

    public GetMyLikedRecipesQueryHandlerTests()
    {
        _mapper = CreateMapper();
        _handler = new GetMyLikedRecipesQueryHandler(_interactionRepository.Object, _mapper, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnLikedRecipesForCurrentUser()
    {
        var userId = Guid.NewGuid();
        QueryOptions<RecipeInteraction>? capturedOptions = null;
        var interaction = BuildInteraction(userId, liked: true, disliked: false, saved: false, "Beğenilen Tarif");
        var resultSet = new Paginate<RecipeInteraction> { Items = [interaction], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _interactionRepository
            .Setup(x => x.GetListAsync(It.IsAny<QueryOptions<RecipeInteraction>>(), It.IsAny<CancellationToken>()))
            .Callback<QueryOptions<RecipeInteraction>, CancellationToken>((options, _) => capturedOptions = options)
            .ReturnsAsync(resultSet);

        var response = await _handler.Handle(new GetMyLikedRecipesQuery { Page = 1, Size = 10 }, CancellationToken.None);

        response.Items.Should().HaveCount(1);
        response.Items[0].Title.Should().Be("Beğenilen Tarif");
        capturedOptions.Should().NotBeNull();
        capturedOptions!.Predicate!.Compile()(interaction).Should().BeTrue();
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static RecipeInteraction BuildInteraction(Guid userId, bool liked, bool disliked, bool saved, string title)
    {
        var recipe = Recipe.Create(Guid.NewGuid(), title, "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);
        recipe.AddImage("https://cdn.example.com/main.jpg", "main-id", true);

        var interaction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, userId);
        if (liked)
            interaction.ToggleLike();
        if (disliked)
            interaction.ToggleDislike();
        if (saved)
            interaction.ToggleSave();

        typeof(RecipeInteraction).GetProperty(nameof(RecipeInteraction.Recipe))!.SetValue(interaction, recipe);
        return interaction;
    }
}

public class GetMyDislikedRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeInteractionRepository> _interactionRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly IMapper _mapper;
    private readonly GetMyDislikedRecipesQueryHandler _handler;

    public GetMyDislikedRecipesQueryHandlerTests()
    {
        _mapper = CreateMapper();
        _handler = new GetMyDislikedRecipesQueryHandler(_interactionRepository.Object, _mapper, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnDislikedRecipesForCurrentUser()
    {
        var userId = Guid.NewGuid();
        QueryOptions<RecipeInteraction>? capturedOptions = null;
        var interaction = BuildInteraction(userId, liked: false, disliked: true, saved: false, "Beğenilmeyen Tarif");
        var resultSet = new Paginate<RecipeInteraction> { Items = [interaction], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _interactionRepository
            .Setup(x => x.GetListAsync(It.IsAny<QueryOptions<RecipeInteraction>>(), It.IsAny<CancellationToken>()))
            .Callback<QueryOptions<RecipeInteraction>, CancellationToken>((options, _) => capturedOptions = options)
            .ReturnsAsync(resultSet);

        var response = await _handler.Handle(new GetMyDislikedRecipesQuery { Page = 1, Size = 10 }, CancellationToken.None);

        response.Items.Should().HaveCount(1);
        response.Items[0].Title.Should().Be("Beğenilmeyen Tarif");
        capturedOptions.Should().NotBeNull();
        capturedOptions!.Predicate!.Compile()(interaction).Should().BeTrue();
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static RecipeInteraction BuildInteraction(Guid userId, bool liked, bool disliked, bool saved, string title)
    {
        var recipe = Recipe.Create(Guid.NewGuid(), title, "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);
        recipe.AddImage("https://cdn.example.com/main.jpg", "main-id", true);

        var interaction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, userId);
        if (liked)
            interaction.ToggleLike();
        if (disliked)
            interaction.ToggleDislike();
        if (saved)
            interaction.ToggleSave();

        typeof(RecipeInteraction).GetProperty(nameof(RecipeInteraction.Recipe))!.SetValue(interaction, recipe);
        return interaction;
    }
}

public class GetMySavedRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeInteractionRepository> _interactionRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly IMapper _mapper;
    private readonly GetMySavedRecipesQueryHandler _handler;

    public GetMySavedRecipesQueryHandlerTests()
    {
        _mapper = CreateMapper();
        _handler = new GetMySavedRecipesQueryHandler(_interactionRepository.Object, _mapper, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnSavedRecipesForCurrentUser()
    {
        var userId = Guid.NewGuid();
        QueryOptions<RecipeInteraction>? capturedOptions = null;
        var interaction = BuildInteraction(userId, liked: false, disliked: false, saved: true, "Kaydedilen Tarif");
        var resultSet = new Paginate<RecipeInteraction> { Items = [interaction], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _interactionRepository
            .Setup(x => x.GetListAsync(It.IsAny<QueryOptions<RecipeInteraction>>(), It.IsAny<CancellationToken>()))
            .Callback<QueryOptions<RecipeInteraction>, CancellationToken>((options, _) => capturedOptions = options)
            .ReturnsAsync(resultSet);

        var response = await _handler.Handle(new GetMySavedRecipesQuery { Page = 1, Size = 10 }, CancellationToken.None);

        response.Items.Should().HaveCount(1);
        response.Items[0].Title.Should().Be("Kaydedilen Tarif");
        capturedOptions.Should().NotBeNull();
        capturedOptions!.Predicate!.Compile()(interaction).Should().BeTrue();
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static RecipeInteraction BuildInteraction(Guid userId, bool liked, bool disliked, bool saved, string title)
    {
        var recipe = Recipe.Create(Guid.NewGuid(), title, "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);
        recipe.AddImage("https://cdn.example.com/main.jpg", "main-id", true);

        var interaction = new RecipeInteraction(Guid.NewGuid(), recipe.Id, userId);
        if (liked)
            interaction.ToggleLike();
        if (disliked)
            interaction.ToggleDislike();
        if (saved)
            interaction.ToggleSave();

        typeof(RecipeInteraction).GetProperty(nameof(RecipeInteraction.Recipe))!.SetValue(interaction, recipe);
        return interaction;
    }
}
