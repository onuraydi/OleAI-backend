using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Features.Users.Queries.GetRecommendation;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Queries.GetRecommendation;

public class GetRecommendationQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IAiRecommendationService> _mockAiService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly UserBusinessRules _userBusinessRules;
    private readonly GetRecommendationQueryHandler _handler;

    public GetRecommendationQueryHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockAiService = new Mock<IAiRecommendationService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _userBusinessRules = new UserBusinessRules(new Mock<IDietaryPreferenceRepository>().Object, new Mock<IAllergyRestrictionRepository>().Object);
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();

        _handler = new GetRecommendationQueryHandler(
            _mockUserRepo.Object,
            _mockAiService.Object,
            _mockHttpContextAccessor.Object,
            _userBusinessRules,
            _mockRecipeRepository.Object,
            _mockMapper.Object,
            _mockIngredientRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenValidUser_ShouldBuildPayloadAndCallAiService()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var dietaryPref = new DietaryPreference { Name = "Vegan" };
        dietaryPref.ExcludedIngredients.Add(new DietaryPreferenceExcludedIngredient { Ingredient = new Ingredient(Guid.NewGuid(), "Süt", Guid.NewGuid()) });

        var allergyRestriction = new AllergyRestriction { Name = "Fındık Alerjisi" };
        allergyRestriction.ExcludedIngredients.Add(new AllergyExcludedIngredient { Ingredient = new Ingredient(Guid.NewGuid(), "Fındık", Guid.NewGuid()) });

        var user = new User
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        };
        user.UserDietaryPreferences.Add(new UserDietaryPreference
        {
            UserId = userId,
            DietaryPreference = dietaryPref
        });
        user.UserAllergies.Add(new UserAllergy
        {
            UserId = userId,
            AllergyRestriction = allergyRestriction
        });

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var recipeId = Guid.NewGuid();
        var expectedResponse = new AiRecommendationResponse
        {
            Recipes = new List<RecommendedRecipeItem>
            {
                new() { RecipeId = recipeId, Score = 0.95 }
            }
        };

        _mockAiService.Setup(s => s.GetRecommendationsAsync(
                It.IsAny<AiRecommendationPayload>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var recipe = Recipe.Create(Guid.NewGuid(), "T", "D", Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, Domain.Enums.RecipeVisibility.Public);
        typeof(OleAI.Backend.Core.Repositories.Entity<Guid>).GetProperty("Id")?.SetValue(recipe, recipeId);

        var recipes = new Paginate<Recipe> { Items = new List<Recipe> { recipe } };
        _mockRecipeRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var mappedRecipes = new Paginate<RecipeListItemResponse> { Items = new List<RecipeListItemResponse> { new RecipeListItemResponse { Id = recipeId } } };
        _mockMapper.Setup(m => m.Map<Paginate<RecipeListItemResponse>>(It.IsAny<Paginate<Recipe>>(), It.IsAny<Action<IMappingOperationOptions<object, Paginate<RecipeListItemResponse>>>>()))
            .Returns(mappedRecipes);

        var staples = new Paginate<Ingredient> { Items = new List<Ingredient>() };
        _mockIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(staples);

        // Act
        var result = await _handler.Handle(new GetRecommendationQuery { Mode = RecommendationMode.Default }, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].MatchScore.Should().Be(0.95);
        result[0].Recipe.Id.Should().Be(recipeId);

        _mockAiService.Verify(s => s.GetRecommendationsAsync(
            It.Is<AiRecommendationPayload>(p =>
                p.UserId == userId &&
                p.DietaryPreferences.Contains("Vegan") &&
                p.ExcludedIngredients.Contains("Fındık")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _handler.Handle(new GetRecommendationQuery { Mode = RecommendationMode.Default }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenNoPreferencesOrAllergies_ShouldSendEmptyPayload()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var user = new User { Id = userId, FirstName = "Test", LastName = "User", Email = "t@t.com" };

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockAiService.Setup(s => s.GetRecommendationsAsync(
                It.IsAny<AiRecommendationPayload>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AiRecommendationResponse { Recipes = new List<RecommendedRecipeItem>() });

        var recipes = new Paginate<Recipe> { Items = new List<Recipe>() };
        _mockRecipeRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var mappedRecipes = new Paginate<RecipeListItemResponse> { Items = new List<RecipeListItemResponse>() };
        _mockMapper.Setup(m => m.Map<Paginate<RecipeListItemResponse>>(It.IsAny<Paginate<Recipe>>(), It.IsAny<Action<IMappingOperationOptions<object, Paginate<RecipeListItemResponse>>>>()))
            .Returns(mappedRecipes);

        var staples = new Paginate<Ingredient> { Items = new List<Ingredient>() };
        _mockIngredientRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Ingredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(staples);

        // Act
        var result = await _handler.Handle(new GetRecommendationQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        _mockAiService.Verify(s => s.GetRecommendationsAsync(
            It.Is<AiRecommendationPayload>(p =>
                p.DietaryPreferences.Count == 0 &&
                p.ExcludedIngredients.Count == 0 &&
                p.ExcludedCategories.Count == 0),
            It.IsAny<CancellationToken>()), Times.Once);
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
