using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.AddComment;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.AddComment;

public class AddCommentCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IRecipeCommentRepository> _mockCommentRepository;
    private readonly Mock<IUserProfileVectorService> _mockVectorService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly RecipeBusinessRules _rules;
    private readonly AddCommentCommandHandler _handler;

    public AddCommentCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockCommentRepository = new Mock<IRecipeCommentRepository>();
        _mockVectorService = new Mock<IUserProfileVectorService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _rules = new RecipeBusinessRules(Mock.Of<IIngredientRepository>(), _mockCommentRepository.Object);

        _handler = new AddCommentCommandHandler(
            _mockRecipeRepository.Object,
            _mockCommentRepository.Object,
            _rules,
            _mockVectorService.Object,
            _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldAddCommentAndReturnMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var recipe = Recipe.Create(userId, "Title", "Desc", OleAI.Backend.Domain.Enums.RecipeDifficulty.Medium, 2, 10, 20, RecipeVisibility.Public);
        recipe.PassLlmFilter();
        recipe.ApproveByAdmin();

        _mockRecipeRepository.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<Recipe, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        var command = new AddCommentCommand { RecipeId = recipe.Id, Content = "Nice!", Rating = 5 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Yorumunuz başarıyla eklendi.");
        _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<RecipeComment>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockVectorService.Verify(v => v.UpdateProfileVectorAsync(userId, recipe.Id, "Comment", It.IsAny<CancellationToken>()), Times.Once);
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

