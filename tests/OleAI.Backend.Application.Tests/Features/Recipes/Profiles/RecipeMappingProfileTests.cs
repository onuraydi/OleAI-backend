using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Profiles;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Profiles;

public class RecipeMappingProfileTests
{
    private readonly IMapper _mapper;

    public RecipeMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<RecipeMappingProfile>());
        var sp = services.BuildServiceProvider();
        _mapper = sp.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Map_RecipeToRecipeListItemResponse_ShouldWork()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);
        recipe.AddImage("main_url", "p1", true);

        // Act
        var result = _mapper.Map<RecipeListItemResponse>(recipe);

        // Assert
        result.Title.Should().Be("Title");
        result.MainImageUrl.Should().Be("main_url");
        result.StatusName.Should().Be(RecipeStatus.PendingLlmApproval.ToString());
    }

    [Fact]
    public void Map_RecipeToRecipeDetailResponse_ShouldIncludeRelatedData()
    {
        // Arrange
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeVisibility.Public);
        recipe.AddIngredient(Guid.NewGuid(), 100, 1);
        recipe.AddStep(1, "Cook it");

        // Act
        var result = _mapper.Map<RecipeDetailResponse>(recipe);

        // Assert
        result.Ingredients.Should().HaveCount(1);
        result.Steps.Should().HaveCount(1);
        result.Steps.First().Instruction.Should().Be("Cook it");
    }
}
