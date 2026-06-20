using System.Linq.Expressions;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Users.Queries.GetUserStatistics;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Queries.GetUserStatistics;

public class GetUserStatisticsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUserStatistics()
    {
        var mockRecipeRepo = new Mock<IRecipeRepository>();
        var mockInteractionRepo = new Mock<IRecipeInteractionRepository>();
        var mockUserIngredientRepo = new Mock<IUserIngredientRepository>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        mockRecipeRepo.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(10);
        mockInteractionRepo.SetupSequence(r => r.GetCountAsync(It.IsAny<Expression<Func<RecipeInteraction, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5)
            .ReturnsAsync(3);
        mockUserIngredientRepo.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<UserIngredient, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(20);

        var handler = new GetUserStatisticsQueryHandler(mockRecipeRepo.Object, mockInteractionRepo.Object, mockUserIngredientRepo.Object, mockHttpContextAccessor.Object);

        var result = await handler.Handle(new GetUserStatisticsQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.TotalRecipesCreated.Should().Be(10);
        result.TotalRecipesSaved.Should().Be(5);
        result.TotalRecipesLiked.Should().Be(3);
        result.TotalPantryItems.Should().Be(20);
    }
}
