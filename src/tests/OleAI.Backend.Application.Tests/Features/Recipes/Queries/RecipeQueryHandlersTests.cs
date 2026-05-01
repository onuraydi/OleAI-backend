using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Profiles;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPendingAdminRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Tests.TestInfrastructure;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries;

public class GetPublishedRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly IMapper _mapper;
    private readonly GetPublishedRecipesQueryHandler _handler;

    public GetPublishedRecipesQueryHandlerTests()
    {
        _mapper = CreateMapper();
        _handler = new GetPublishedRecipesQueryHandler(_recipeRepository.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WhenSearchTermIsNotProvided_ShouldReturnPublishedPublicRecipes()
    {
        QueryOptions<Recipe>? capturedOptions = null;
        var recipe = BuildPublishedRecipe("Mercimek Çorbası", "Lezzetli çorba");
        var resultSet = new Paginate<Recipe> { Items = [recipe], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _recipeRepository
            .Setup(x => x.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .Callback<QueryOptions<Recipe>, CancellationToken>((options, _) => capturedOptions = options)
            .ReturnsAsync(resultSet);

        var response = await _handler.Handle(new GetPublishedRecipesQuery { Page = 2, Size = 5 }, CancellationToken.None);

        response.Items.Should().HaveCount(1);
        response.Items[0].Title.Should().Be("Mercimek Çorbası");
        capturedOptions.Should().NotBeNull();
        capturedOptions!.Index.Should().Be(2);
        capturedOptions.Size.Should().Be(5);
        capturedOptions.EnableTracking.Should().BeFalse();
        capturedOptions.Predicate!.Compile()(recipe).Should().BeTrue();
        capturedOptions.Predicate!.Compile()(Recipe.Create(Guid.NewGuid(), "Private", "Hidden", RecipeVisibility.Private)).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenSearchTermIsProvided_ShouldUseSearchPredicate()
    {
        QueryOptions<Recipe>? capturedOptions = null;
        var matchingRecipe = BuildPublishedRecipe("Mercimek Çorbası", "Sıcak ve kolay");
        var nonMatchingRecipe = BuildPublishedRecipe("Tavuk Suyu", "Açık çorba");
        var resultSet = new Paginate<Recipe> { Items = [matchingRecipe], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _recipeRepository
            .Setup(x => x.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .Callback<QueryOptions<Recipe>, CancellationToken>((options, _) => capturedOptions = options)
            .ReturnsAsync(resultSet);

        var response = await _handler.Handle(new GetPublishedRecipesQuery { SearchTerm = "çorba" }, CancellationToken.None);

        response.Items.Should().HaveCount(1);
        capturedOptions.Should().NotBeNull();
        capturedOptions!.Predicate!.Compile()(matchingRecipe).Should().BeTrue();
        capturedOptions.Predicate!.Compile()(nonMatchingRecipe).Should().BeFalse();
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static Recipe BuildPublishedRecipe(string title, string description)
    {
        var recipe = Recipe.Create(Guid.NewGuid(), title, description, RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);
        recipe.AddImage("https://cdn.example.com/main.jpg", "main-id", true);
        return recipe;
    }
}

public class GetPendingAdminRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly IMapper _mapper;
    private readonly GetPendingAdminRecipesQueryHandler _handler;

    public GetPendingAdminRecipesQueryHandlerTests()
    {
        _mapper = CreateMapper();
        _handler = new GetPendingAdminRecipesQueryHandler(_recipeRepository.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnPendingAdminRecipes()
    {
        QueryOptions<Recipe>? capturedOptions = null;
        var pendingRecipe = Recipe.Create(Guid.NewGuid(), "Bekleyen", "Açıklama", RecipeVisibility.Public);
        var resultSet = new Paginate<Recipe> { Items = [pendingRecipe], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _recipeRepository
            .Setup(x => x.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .Callback<QueryOptions<Recipe>, CancellationToken>((options, _) => capturedOptions = options)
            .ReturnsAsync(resultSet);

        var response = await _handler.Handle(new GetPendingAdminRecipesQuery { Page = 3, Size = 15 }, CancellationToken.None);

        response.Items.Should().HaveCount(1);
        response.Items[0].Title.Should().Be("Bekleyen");
        capturedOptions.Should().NotBeNull();
        capturedOptions!.Index.Should().Be(3);
        capturedOptions.Size.Should().Be(15);
        capturedOptions.EnableTracking.Should().BeFalse();
        capturedOptions.Predicate!.Compile()(pendingRecipe).Should().BeFalse();

        var pendingAdminRecipe = Recipe.Create(Guid.NewGuid(), "Bekleyen", "Açıklama", RecipeVisibility.Public);
        pendingAdminRecipe.PassLlmFilter();
        capturedOptions.Predicate!.Compile()(pendingAdminRecipe).Should().BeTrue();
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
}

public class GetMyRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly IMapper _mapper;
    private readonly GetMyRecipesQueryHandler _handler;

    public GetMyRecipesQueryHandlerTests()
    {
        _mapper = CreateMapper();
        _handler = new GetMyRecipesQueryHandler(_recipeRepository.Object, _mapper, _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnOnlyCurrentUsersRecipes()
    {
        var userId = Guid.NewGuid();
        QueryOptions<Recipe>? capturedOptions = null;
        var userRecipe = BuildPublishedRecipe(userId, "Kişisel Tarif");
        var otherRecipe = BuildPublishedRecipe(Guid.NewGuid(), "Başka Tarif");
        var resultSet = new Paginate<Recipe> { Items = [userRecipe], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(RecipeTestHelpers.CreateHttpContext(userId));
        _recipeRepository
            .Setup(x => x.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .Callback<QueryOptions<Recipe>, CancellationToken>((options, _) => capturedOptions = options)
            .ReturnsAsync(resultSet);

        var response = await _handler.Handle(new GetMyRecipesQuery { Page = 1, Size = 20 }, CancellationToken.None);

        response.Items.Should().HaveCount(1);
        response.Items[0].Title.Should().Be("Kişisel Tarif");
        capturedOptions.Should().NotBeNull();
        capturedOptions!.Predicate!.Compile()(userRecipe).Should().BeTrue();
        capturedOptions.Predicate!.Compile()(otherRecipe).Should().BeFalse();
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static Recipe BuildPublishedRecipe(Guid ownerId, string title)
    {
        var recipe = Recipe.Create(ownerId, title, "Açıklama", RecipeVisibility.Public);
        recipe.PassLlmFilter();
        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);
        recipe.AddImage($"https://cdn.example.com/{title}.jpg", "main-id", true);
        return recipe;
    }
}
