using System.Reflection;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Queries.GetCategorizedPopularIngredients;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Queries.GetCategorizedPopularIngredients;

public class GetCategorizedPopularIngredientsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCategorizedPopularIngredients()
    {
        var mockRepo = new Mock<ICategoryRepository>();
        var mockCache = new Mock<ICacheService>();

        mockCache.Setup(c => c.GetAsync<List<OleAI.Backend.Application.Features.Pantry.Dtos.Responses.CategorizedPopularIngredientsResponse>>(It.IsAny<string>()))
            .ReturnsAsync((List<OleAI.Backend.Application.Features.Pantry.Dtos.Responses.CategorizedPopularIngredientsResponse>)null);

        var catId = Guid.NewGuid();
        var cat = new Category(catId, "Cat1", "Icon");

        var ingredient = new Ingredient(Guid.NewGuid(), "Ing", catId, OleAI.Backend.Domain.Enums.MeasurementCategory.Weight, null, OleAI.Backend.Domain.Enums.ExpirationMode.Auto, false);
        var ingredientsList = new HashSet<Ingredient> { ingredient };

        var categoryProp = typeof(Category).GetProperty("Ingredients", BindingFlags.Public | BindingFlags.Instance);
        categoryProp.SetValue(cat, ingredientsList);

        var categories = new Paginate<Category>
        {
            Items = new List<Category> { cat }
        };

        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Category>>(), It.IsAny<CancellationToken>())).ReturnsAsync(categories);

        var handler = new GetCategorizedPopularIngredientsQueryHandler(mockRepo.Object, mockCache.Object);

        var result = await handler.Handle(new GetCategorizedPopularIngredientsQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }
}
