using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Profiles;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Profiles;

public class RecipeMappingProfileTests
{
    private readonly IMapper _mapper;

    public RecipeMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact]
    public void Construct_ShouldBuildValidMappings()
    {
        var recipe = BuildRecipe();
        var paginate = new Paginate<Recipe> { Items = [recipe], PageNumber = 1, PageSize = 10, TotalItems = 1, TotalPages = 1 };

        Action act = () =>
        {
            _ = _mapper.Map<PendingAdminRecipeListResponse>(recipe);
            _ = _mapper.Map<RecipeListItemResponse>(recipe);
            _ = _mapper.Map<RecipeDetailResponse>(recipe);
            _ = _mapper.Map<Paginate<RecipeListItemResponse>>(paginate);
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void Map_Recipe_To_RecipeListItemResponse_ShouldMapMainImageAndStatus()
    {
        var recipe = BuildRecipe();

        var response = _mapper.Map<RecipeListItemResponse>(recipe);

        response.Id.Should().Be(recipe.Id);
        response.Title.Should().Be(recipe.Title);
        response.MainImageUrl.Should().Be("https://cdn.example.com/main.jpg");
        response.StatusName.Should().Be(recipe.Status.ToString());
    }

    [Fact]
    public void Map_Recipe_To_RecipeDetailResponse_ShouldMapNestedCollections()
    {
        var recipe = BuildRecipe();

        var response = _mapper.Map<RecipeDetailResponse>(recipe);

        response.Id.Should().Be(recipe.Id);
        response.Ingredients.Should().HaveCount(1);
        response.Steps.Should().HaveCount(1);
        response.Images.Should().HaveCount(2);
    }

    [Fact]
    public void Map_Recipe_To_PendingAdminRecipeListResponse_ShouldMapCoreFields()
    {
        var recipe = BuildRecipe();

        var response = _mapper.Map<PendingAdminRecipeListResponse>(recipe);

        response.Id.Should().Be(recipe.Id);
        response.Title.Should().Be(recipe.Title);
        response.Description.Should().Be(recipe.Description);
    }

    private static Recipe BuildRecipe()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "Mantı", "Lezzetli tarif", RecipeVisibility.Public);
        recipe.PassLlmFilter();

        typeof(Recipe).GetProperty(nameof(Recipe.Status))!.SetValue(recipe, RecipeStatus.Published);

        recipe.AddIngredient(Guid.NewGuid(), 2, 1);
        recipe.AddStep(1, "Hamuru aç");
        recipe.AddImage("https://cdn.example.com/main.jpg", "main-public-id", true);
        recipe.AddImage("https://cdn.example.com/step-1.jpg", "step-public-id", false, recipe.RecipeSteps.First().Id);

        return recipe;
    }
}
