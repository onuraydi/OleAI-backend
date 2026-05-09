using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Users.Queries.GetRecommendation;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Queries.GetRecommendation;

public class GetRecommendationQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IAiRecommendationService> _mockAiService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly UserBusinessRules _userBusinessRules;
    private readonly GetRecommendationQueryHandler _handler;

    public GetRecommendationQueryHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockAiService = new Mock<IAiRecommendationService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        _userBusinessRules = new UserBusinessRules(
            Mock.Of<IDietaryPreferenceRepository>(),
            Mock.Of<IAllergyRestrictionRepository>());

        _handler = new GetRecommendationQueryHandler(
            _mockUserRepo.Object,
            _mockAiService.Object,
            _mockHttpContextAccessor.Object,
            _userBusinessRules);
    }

    [Fact]
    public async Task Handle_WhenValidUser_ShouldBuildPayloadAndCallAiService()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var dietaryPref = new DietaryPreference { Name = "Vegan" };
        var allergyRestriction = new AllergyRestriction { Name = "Fındık Alerjisi" };

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

        var expectedResponse = new AiRecommendationResponse
        {
            Recipes = new List<RecommendedRecipeItem>
            {
                new() { RecipeId = Guid.NewGuid(), Title = "Vegan Salata", Score = 0.95 }
            }
        };

        _mockAiService.Setup(s => s.GetRecommendationsAsync(
                It.IsAny<AiRecommendationPayload>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.Handle(new GetRecommendationQuery(), CancellationToken.None);

        // Assert
        result.Recipes.Should().HaveCount(1);
        result.Recipes[0].Title.Should().Be("Vegan Salata");

        _mockAiService.Verify(s => s.GetRecommendationsAsync(
            It.Is<AiRecommendationPayload>(p =>
                p.UserId == userId &&
                p.DietaryPreferences.Contains("Vegan") &&
                p.Allergies.Contains("Fındık Alerjisi")),
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
        var act = () => _handler.Handle(new GetRecommendationQuery(), CancellationToken.None);

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
            .ReturnsAsync(new AiRecommendationResponse());

        // Act
        var result = await _handler.Handle(new GetRecommendationQuery(), CancellationToken.None);

        // Assert
        result.Recipes.Should().BeEmpty();

        _mockAiService.Verify(s => s.GetRecommendationsAsync(
            It.Is<AiRecommendationPayload>(p =>
                p.DietaryPreferences.Count == 0 &&
                p.Allergies.Count == 0 &&
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
