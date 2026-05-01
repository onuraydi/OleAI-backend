using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
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
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var recipe = Recipe.Create(userId, "Title", "Desc", RecipeVisibility.Public);
        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _mockUserIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<OleAI.Backend.Core.Repositories.Options.QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OleAI.Backend.Core.Repositories.Pagination.Paginate<UserIngredient>());

        var command = new StartCookingCommand { RecipeId = recipe.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();
        _mockRecipeRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(), It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
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
