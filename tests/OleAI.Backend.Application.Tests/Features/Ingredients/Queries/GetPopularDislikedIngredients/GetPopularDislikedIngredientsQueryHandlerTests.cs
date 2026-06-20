using System.Reflection;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetPopularDislikedIngredients;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Queries.GetPopularDislikedIngredients;

public class GetPopularDislikedIngredientsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPopularDislikedIngredients()
    {
        var mockRepo = new Mock<IIngredientRepository>();

        var id = Guid.NewGuid();
        var category = new Category(Guid.NewGuid(), "Cat", "Desc");
        var ingredient = new Ingredient(id, "Test", category.Id, OleAI.Backend.Domain.Enums.MeasurementCategory.Weight, null, OleAI.Backend.Domain.Enums.ExpirationMode.Auto, false);

        // Use reflection to set category
        var categoryProp = typeof(Ingredient).GetProperty("Category", BindingFlags.Public | BindingFlags.Instance);
        categoryProp.SetValue(ingredient, category);

        var ingredients = new Paginate<Ingredient> { Items = new List<Ingredient> { ingredient } };

        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>())).ReturnsAsync(ingredients);

        var handler = new GetPopularDislikedIngredientsQueryHandler(mockRepo.Object);

        var result = await handler.Handle(new GetPopularDislikedIngredientsQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Test");
    }
}
