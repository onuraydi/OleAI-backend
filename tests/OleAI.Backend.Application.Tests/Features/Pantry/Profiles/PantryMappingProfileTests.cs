using System.Reflection;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Profiles;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Profiles;

public class PantryMappingProfileTests
{
    private readonly IMapper _mapper;

    public PantryMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<PantryMappingProfile>());
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Construct_ShouldHaveValidConfiguration()
    {
        var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2);
        var ingredientRequest = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Domat");

        var act1 = () => _mapper.Map<PantryItemResponse>(userIngredient);
        var act2 = () => _mapper.Map<ResolvedRequestsResponse>(ingredientRequest);

        act1.Should().NotThrow();
        act2.Should().NotThrow();
    }

    [Fact]
    public void Map_UserIngredient_WithMasterIngredient_ShouldMapNameCategoryUnitAndPendingFlag()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Sebze");
        var ingredient = new Ingredient(Guid.NewGuid(), "Domates", category.Id, "adet");
        SetPrivateProperty(ingredient, nameof(Ingredient.Category), category);

        var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), ingredient.Id, 3);
        SetPrivateProperty(userIngredient, nameof(UserIngredient.Ingredient), ingredient);

        // Act
        var result = _mapper.Map<PantryItemResponse>(userIngredient);

        // Assert
        result.MasterIngredientId.Should().Be(ingredient.Id);
        result.Name.Should().Be("Domates");
        result.CategoryName.Should().Be("Sebze");
        result.Unit.Should().Be("adet");
        result.IsPendingApproval.Should().BeFalse();
    }

    [Fact]
    public void Map_UserIngredient_WithPendingRequest_ShouldUseRequestedNameAndPendingFlag()
    {
        // Arrange
        var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Yeni Ürün");
        var userIngredient = UserIngredient.CreatePending(Guid.NewGuid(), Guid.NewGuid(), request.Id, 1);
        SetPrivateProperty(userIngredient, nameof(UserIngredient.IngredientRequest), request);

        // Act
        var result = _mapper.Map<PantryItemResponse>(userIngredient);

        // Assert
        result.MasterIngredientId.Should().BeNull();
        result.Name.Should().Be("Yeni Ürün");
        result.IsPendingApproval.Should().BeTrue();
    }

    [Fact]
    public void Map_UserIngredient_WithMissingRelations_ShouldUseFallbackValues()
    {
        // Arrange
        var userIngredient = UserIngredient.CreatePending(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);

        // Act
        var result = _mapper.Map<PantryItemResponse>(userIngredient);

        // Assert
        result.Name.Should().Be("Bilinmeyen Ürün");
        result.CategoryName.Should().Be("Kategorisiz");
        result.Unit.Should().Be("Adet/Gr");
    }

    private static void SetPrivateProperty<T>(T target, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.Should().NotBeNull($"{propertyName} özelliği bulunmalı");
        property!.SetValue(target, value);
    }
}
