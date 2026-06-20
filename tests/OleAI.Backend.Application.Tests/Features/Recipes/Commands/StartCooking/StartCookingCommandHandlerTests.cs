using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.StartCooking;

public class StartCookingCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _recipeBusinessRules;
    private readonly Mock<IMapper> _mockMapper;
    private readonly StartCookingCommandHandler _handler;

    public StartCookingCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockCommentRepo = new Mock<IRecipeCommentRepository>();
        _recipeBusinessRules = new RecipeBusinessRules(_mockIngredientRepository.Object, mockCommentRepo.Object);
        _mockMapper = new Mock<IMapper>();

        _handler = new StartCookingCommandHandler(
            _mockRecipeRepository.Object,
            _mockUserIngredientRepository.Object,
            _mockIngredientRepository.Object,
            _mockHttpContextAccessor.Object,
            _recipeBusinessRules,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var command = new StartCookingCommand { RecipeId = Guid.NewGuid() };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldDeductIngredientsAndReturnDepleted()
    {
        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var recipeId = Guid.NewGuid();
        var command = new StartCookingCommand { RecipeId = recipeId, IgnoredIngredientIds = new List<Guid>() };

        var ingredient1 = new Ingredient(Guid.NewGuid(), "Ing1", Guid.NewGuid(), OleAI.Backend.Domain.Enums.MeasurementCategory.Count);
        var recipe = Recipe.Create(userId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 10, 10, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);
        recipe.AddIngredient(ingredient1.Id, null, 2, 0);
        // 0 corresponds to a valid unit id (Piece) for Count. We manually set it.
        var recipeIngredient = recipe.RecipeIngredients.First();
        recipeIngredient.Ingredient = ingredient1;

        _mockRecipeRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(),
            It.IsAny<Func<IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredient1.Id, 2, OleAI.Backend.Domain.Enums.MeasurementUnit.Piece);

        var userIngredientsResult = new Paginate<UserIngredient> { Items = new List<UserIngredient> { userIngredient } };
        _mockUserIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userIngredientsResult);

        var dbIngredients = new Paginate<Ingredient> { Items = new List<Ingredient> { ingredient1 } };
        _mockIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbIngredients);

        var ingredientResponses = new List<IngredientResponse> { new IngredientResponse { Id = ingredient1.Id, Name = "Ing1" } };
        _mockMapper.Setup(m => m.Map<List<IngredientResponse>>(It.IsAny<IList<Ingredient>>())).Returns(ingredientResponses);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _mockUserIngredientRepository.Verify(x => x.DeleteAsync(It.Is<UserIngredient>(ui => ui.Id == userIngredient.Id), true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
