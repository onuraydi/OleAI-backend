using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Profiles;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Profiles;

public class CategoryMappingProfileTests
{
    private readonly IMapper _mapper;

    public CategoryMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<CategoryMappingProfile>());
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Construct_ShouldHaveValidConfiguration()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Bakliyat");
        var createRequest = new CreateCategoryRequest { Name = "Baharat" };
        var updateRequest = new UpdateCategoryRequest { Id = category.Id, Name = "Güncel" };

        // Act
        var act1 = () => _mapper.Map<CategoryResponse>(category);
        var act2 = () => _mapper.Map<CreatedCategoryResponse>(category);
        var act3 = () => _mapper.Map<Category>(createRequest);
        var act4 = () => _mapper.Map(updateRequest, category);

        // Assert
        act1.Should().NotThrow();
        act2.Should().NotThrow();
        act3.Should().NotThrow();
        act4.Should().NotThrow();
    }

    [Fact]
    public void Map_Category_To_CategoryResponse_ShouldMapCorrectly()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Bakliyat");

        // Act
        var response = _mapper.Map<CategoryResponse>(category);

        // Assert
        response.Id.Should().Be(category.Id);
        response.Name.Should().Be(category.Name);
    }

    [Fact]
    public void Map_Category_To_CreatedCategoryResponse_ShouldMapCorrectly()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Baharat");

        // Act
        var response = _mapper.Map<CreatedCategoryResponse>(category);

        // Assert
        response.Id.Should().Be(category.Id);
        response.Name.Should().Be(category.Name);
    }
}
