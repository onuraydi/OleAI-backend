using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries;

public class DummyQuery : IRequest<Paginate<RecipeListItemResponse>>, IPageableRequest
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}

public class DummyQueryHandler : BaseUserInteractionRecipesQueryHandler<DummyQuery>
{
    public DummyQueryHandler(IRecipeInteractionRepository repo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        : base(repo, mapper, httpContextAccessor) { }

    protected override Expression<Func<RecipeInteraction, bool>> GetInteractionPredicate(Guid userId, DummyQuery request)
    {
        return i => i.UserId == userId;
    }
}

public class BaseUserInteractionRecipesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPaginatedList()
    {
        var mockRepo = new Mock<IRecipeInteractionRepository>();
        var mockMapper = new Mock<IMapper>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var interaction = new RecipeInteraction();
        var recipe = Recipe.Create(Guid.NewGuid(), "T", "D", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);

        typeof(RecipeInteraction).GetProperty("Id")?.SetValue(interaction, Guid.NewGuid());
        typeof(RecipeInteraction).GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance)?.SetValue(interaction, userId);
        typeof(RecipeInteraction).GetProperty("RecipeId", BindingFlags.Public | BindingFlags.Instance)?.SetValue(interaction, Guid.NewGuid());
        typeof(RecipeInteraction).GetProperty("Recipe", BindingFlags.Public | BindingFlags.Instance)?.SetValue(interaction, recipe);
        var paginate = new Paginate<RecipeInteraction> { Items = new List<RecipeInteraction> { interaction }, TotalItems = 1 };
        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<RecipeInteraction>>(), It.IsAny<CancellationToken>())).ReturnsAsync(paginate);

        var dtoList = new List<RecipeListItemResponse> { new RecipeListItemResponse { Id = recipe.Id, Title = "T" } };
        mockMapper.Setup(m => m.Map<IList<RecipeListItemResponse>>(It.IsAny<List<Recipe>>(), It.IsAny<Action<IMappingOperationOptions<object, IList<RecipeListItemResponse>>>>())).Returns(dtoList);

        var handler = new DummyQueryHandler(mockRepo.Object, mockMapper.Object, mockHttpContextAccessor.Object);

        var result = await handler.Handle(new DummyQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }
}
