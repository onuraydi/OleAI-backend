using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Profiles;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Profiles;

public class IngredientMappingProfileTests
{
    private readonly IMapper _mapper;

    public IngredientMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<IngredientMappingProfile>());
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Construct_ShouldHaveValidConfiguration()
    {
        // Arrange
        var ingredient = new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), "gram");
        var createRequest = new CreateIngredientRequest { Name = "Karabiber", CategoryId = Guid.NewGuid(), BaseUnitType = "gram" };
        var updateRequest = new UpdateIngredientRequest { Id = ingredient.Id, Name = "Yeni", CategoryId = Guid.NewGuid(), BaseUnitType = "adet" };

        // Act
        var act1 = () => _mapper.Map<CreatedIngredientResponse>(ingredient);
        var act2 = () => _mapper.Map<Ingredient>(createRequest);
        var act3 = () => _mapper.Map(updateRequest, ingredient);

        // Assert
        act1.Should().NotThrow();
        act2.Should().NotThrow();
        act3.Should().NotThrow();
    }

    [Fact]
    public void Map_CreateIngredientRequest_To_Ingredient_ShouldMapCorrectly()
    {
        // Arrange
        var request = new CreateIngredientRequest
        {
            Name = "Tuz",
            CategoryId = Guid.NewGuid(),
            BaseUnitType = "gram"
        };

        // Act
        var ingredient = _mapper.Map<Ingredient>(request);

        // Assert
        ingredient.Name.Should().Be(request.Name);
        ingredient.CategoryId.Should().Be(request.CategoryId);
        ingredient.BaseUnitType.Should().Be(request.BaseUnitType);
    }

    [Fact]
    public void Map_Ingredient_To_CreatedIngredientResponse_ShouldMapCorrectly()
    {
        // Arrange
        var ingredient = new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), "gram");

        // Act
        var response = _mapper.Map<CreatedIngredientResponse>(ingredient);

        // Assert
        response.Id.Should().Be(ingredient.Id);
        response.Name.Should().Be(ingredient.Name);
    }
}
