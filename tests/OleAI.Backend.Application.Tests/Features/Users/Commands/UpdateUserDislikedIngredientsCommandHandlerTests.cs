using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserDislikedIngredients;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Commands.UpdateUserDislikedIngredients;

public class UpdateUserDislikedIngredientsCommandHandlerTests
{
    private readonly Mock<IUserDislikedIngredientRepository> _mockRepository;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IUserProfileVectorService> _mockVectorService;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly IngredientBusinessRules _ingredientRules;
    private readonly UpdateUserDislikedIngredientsCommandHandler _handler;

    public UpdateUserDislikedIngredientsCommandHandlerTests()
    {
        _mockRepository = new Mock<IUserDislikedIngredientRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCacheService = new Mock<ICacheService>();
        _mockVectorService = new Mock<IUserProfileVectorService>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();

        _ingredientRules = new IngredientBusinessRules(_mockIngredientRepository.Object);

        _handler = new UpdateUserDislikedIngredientsCommandHandler(
            _mockRepository.Object,
            _mockHttpContextAccessor.Object,
            _mockCacheService.Object,
            _mockVectorService.Object,
            _ingredientRules);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldUpdateAndReturnSuccessMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var command = new UpdateUserDislikedIngredientsCommand
        {
            IngredientIds = new List<Guid> { ingredientId }
        };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var mockHttpContext = new DefaultHttpContext { User = claimsPrincipal };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

        _mockIngredientRepository
            .Setup(r => r.GetCountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Ingredient, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockRepository
            .Setup(r => r.ExecuteDeleteAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserDislikedIngredient, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockRepository
            .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<UserDislikedIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDislikedIngredient>());

        _mockCacheService
            .Setup(c => c.RemoveAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockVectorService
            .Setup(v => v.UpdateDietaryProfileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Sevilmeyen malzemeleriniz başarıyla güncellendi.");
        _mockCacheService.Verify(c => c.RemoveAsync($"user-profile:{userId}"), Times.Once);
        _mockVectorService.Verify(v => v.UpdateDietaryProfileAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
