using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.StartCooking;

public class StartCookingCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListItemRepository;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly StartCookingCommandHandler _handler;

    public StartCookingCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockShoppingListItemRepository = new Mock<IShoppingListItemRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), Mock.Of<IRecipeCommentRepository>());

        _handler = new StartCookingCommandHandler(
            _mockRecipeRepository.Object,
            _mockUserIngredientRepository.Object,
            _mockShoppingListItemRepository.Object,
            _mockHttpContextAccessor.Object,
            _rules);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldReturnSuccessMessage()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);
        var recipe = Recipe.Create(userId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _mockUserIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<OleAI.Backend.Core.Repositories.Options.QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OleAI.Backend.Core.Repositories.Pagination.Paginate<UserIngredient> { Items = new List<UserIngredient>() });

        _mockShoppingListItemRepository.Setup(r => r.GetListAsync(It.IsAny<OleAI.Backend.Core.Repositories.Options.QueryOptions<ShoppingListItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OleAI.Backend.Core.Repositories.Pagination.Paginate<ShoppingListItem> { Items = new List<ShoppingListItem>() });

        var result = await _handler.Handle(new StartCookingCommand { RecipeId = recipe.Id }, CancellationToken.None);

        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenIngredientsMissing_ShouldAddToShoppingList()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);
        var recipe = Recipe.Create(userId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);
        recipe.AddIngredient(Guid.NewGuid(), null, 100, 1); // 100g of something
        recipe.AddStep(1, "Start the process");
        recipe.AddStep(2, "Finish the process");

        _mockRecipeRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(), It.IsAny<Func<IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _mockUserIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<OleAI.Backend.Core.Repositories.Options.QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OleAI.Backend.Core.Repositories.Pagination.Paginate<UserIngredient> { Items = new List<UserIngredient>() }); // Empty pantry

        _mockShoppingListItemRepository.Setup(r => r.GetListAsync(It.IsAny<OleAI.Backend.Core.Repositories.Options.QueryOptions<ShoppingListItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OleAI.Backend.Core.Repositories.Pagination.Paginate<ShoppingListItem> { Items = new List<ShoppingListItem>() });

        await _handler.Handle(new StartCookingCommand { RecipeId = recipe.Id }, CancellationToken.None);

        _mockShoppingListItemRepository.Verify(r => r.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupUser(Guid userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);
    }
}

