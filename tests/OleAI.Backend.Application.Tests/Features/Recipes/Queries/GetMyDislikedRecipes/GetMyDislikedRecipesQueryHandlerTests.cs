using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetMyDislikedRecipes;

public class GetMyDislikedRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeInteractionRepository> _mockInteractionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly GetMyDislikedRecipesQueryHandler _handler;

    public GetMyDislikedRecipesQueryHandlerTests()
    {
        _mockInteractionRepository = new Mock<IRecipeInteractionRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _handler = new GetMyDislikedRecipesQueryHandler(_mockInteractionRepository.Object, _mockMapper.Object, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnDislikedRecipes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var query = new GetMyDislikedRecipesQuery { Page = 0, Size = 10 };
        var interaction = new RecipeInteraction(Guid.NewGuid(), userId, Guid.NewGuid());
        interaction.ToggleDislike(); // IsDisliked = true

        var interactions = new Paginate<RecipeInteraction> { Items = new List<RecipeInteraction> { interaction } };

        _mockInteractionRepository.Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<RecipeInteraction>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(interactions);

        var response = new List<RecipeListItemResponse> { new() { Title = "Disliked" } };
        _mockMapper.Setup(m => m.Map<IList<RecipeListItemResponse>>(It.IsAny<List<Recipe>>(), It.IsAny<Action<IMappingOperationOptions<object, IList<RecipeListItemResponse>>>>())).Returns(response);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
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
