using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMissingIngredients;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetMissingIngredients;

public class GetMissingIngredientsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMissingIngredients()
    {
        var mockRecipeRepo = new Mock<IRecipeRepository>();
        var mockUserIngredientRepo = new Mock<IUserIngredientRepository>();
        var mockIngredientRepo = new Mock<IIngredientRepository>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockMapper = new Mock<IMapper>();

        var userId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var recipe = Recipe.Create(recipeId, "T", "D", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);

        mockRecipeRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(), false, false, It.IsAny<CancellationToken>())).ReturnsAsync(recipe);

        var userIngredients = new Paginate<UserIngredient> { Items = new List<UserIngredient>() };
        mockUserIngredientRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>())).ReturnsAsync(userIngredients);

        mockMapper.Setup(m => m.Map<List<OleAI.Backend.Application.Features.Ingredients.Dtos.Responses.IngredientResponse>>(It.IsAny<List<Ingredient>>()))
            .Returns(new List<OleAI.Backend.Application.Features.Ingredients.Dtos.Responses.IngredientResponse>());

        var handler = new GetMissingIngredientsQueryHandler(mockRecipeRepo.Object, mockUserIngredientRepo.Object, mockIngredientRepo.Object, mockHttpContextAccessor.Object, mockMapper.Object);

        var result = await handler.Handle(new GetMissingIngredientsQuery { RecipeId = recipeId }, CancellationToken.None);

        result.Should().NotBeNull();
    }
}
