using System.Security.Claims;
using AutoMapper;
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
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly StartCookingCommandHandler _handler;

    public StartCookingCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), Mock.Of<IRecipeCommentRepository>());

        _handler = new StartCookingCommandHandler(
            _mockRecipeRepository.Object,
            _mockUserIngredientRepository.Object,
            _mockIngredientRepository.Object,
            _mockHttpContextAccessor.Object,
            _rules,
            _mockMapper.Object);
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

        var result = await _handler.Handle(new StartCookingCommand { RecipeId = recipe.Id }, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        _mockHttpContextAccessor.Setup(x => x.HttpContext!.User.FindFirst(It.IsAny<string>()))
            .Returns((Claim?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(new StartCookingCommand { RecipeId = Guid.NewGuid() }, CancellationToken.None));
    }

    private void SetupUser(Guid userId)
    {
        _mockHttpContextAccessor.Setup(x => x.HttpContext!.User.FindFirst(It.IsAny<string>()))
            .Returns(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
    }
}
