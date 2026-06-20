using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IRecipeInteractionRepository> _interactionRepositoryMock;
    private readonly Mock<IRecipeCommentRepository> _commentRepositoryMock;
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock;
    private readonly Mock<IIngredientRequestRepository> _ingredientRequestRepositoryMock;
    private readonly Mock<IAdminStatsRepository> _adminStatsRepositoryMock;
    private readonly GetDashboardStatsQueryHandler _handler;

    public GetDashboardStatsQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _interactionRepositoryMock = new Mock<IRecipeInteractionRepository>();
        _commentRepositoryMock = new Mock<IRecipeCommentRepository>();
        _ingredientRepositoryMock = new Mock<IIngredientRepository>();
        _ingredientRequestRepositoryMock = new Mock<IIngredientRequestRepository>();
        _adminStatsRepositoryMock = new Mock<IAdminStatsRepository>();

        _handler = new GetDashboardStatsQueryHandler(
            _userRepositoryMock.Object,
            _recipeRepositoryMock.Object,
            _interactionRepositoryMock.Object,
            _commentRepositoryMock.Object,
            _ingredientRepositoryMock.Object,
            _ingredientRequestRepositoryMock.Object,
            _adminStatsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectStats()
    {
        // Arrange
        var query = new GetDashboardStatsQuery();

        _userRepositoryMock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
        _recipeRepositoryMock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(50);
        _commentRepositoryMock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<RecipeComment, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(200);
        _ingredientRepositoryMock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(300);

        _interactionRepositoryMock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<RecipeInteraction, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(150);
        _ingredientRequestRepositoryMock.Setup(x => x.GetCountAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(20);

        _adminStatsRepositoryMock.Setup(x => x.GetUserGrowthAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyStatPoint> { new DailyStatPoint { Date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd"), Count = 5 } });
        _adminStatsRepositoryMock.Setup(x => x.GetRecipeGrowthAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyStatPoint> { new DailyStatPoint { Date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd"), Count = 2 } });
        _adminStatsRepositoryMock.Setup(x => x.GetRecentActivitiesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RecentActivityDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalUsers.Should().Be(100);
        result.TotalRecipes.Should().Be(50);
        result.TotalComments.Should().Be(200);
        result.TotalIngredients.Should().Be(300);

        // Ensure calls were made
        _userRepositoryMock.Verify(x => x.GetCountAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        _recipeRepositoryMock.Verify(x => x.GetCountAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Exactly(8)); // 1 total + 4 statuses + 3 difficulties
        _interactionRepositoryMock.Verify(x => x.GetCountAsync(It.IsAny<Expression<Func<RecipeInteraction, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Exactly(3)); // likes, saves, dislikes
    }
}
