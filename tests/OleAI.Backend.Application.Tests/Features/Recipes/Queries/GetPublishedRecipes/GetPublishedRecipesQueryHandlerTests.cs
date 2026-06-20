using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetPublishedRecipes;

public class GetPublishedRecipesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPublishedRecipes()
    {
        var mockRepo = new Mock<IRecipeRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockCache = new Mock<ICacheService>();
        var mockMapper = new Mock<IMapper>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var handler = new GetPublishedRecipesQueryHandler(mockRepo.Object, mockUserRepo.Object, mockCache.Object, mockMapper.Object, mockHttpContextAccessor.Object);

        var request = new GetPublishedRecipesQuery { Page = 1, Size = 10 };
        var recipe = Recipe.Create(Guid.NewGuid(), "T", "D", OleAI.Backend.Domain.Enums.RecipeDifficulty.Easy, 1, 1, 1, OleAI.Backend.Domain.Enums.RecipeVisibility.Public);
        var paginatedList = new Paginate<Recipe> { Items = new List<Recipe> { recipe }, TotalItems = 1 };

        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>())).ReturnsAsync(paginatedList);

        var dto = new RecipeListItemResponse { Id = recipe.Id, Title = "T" };
        var paginatedDtos = new Paginate<RecipeListItemResponse> { Items = new List<RecipeListItemResponse> { dto }, TotalItems = 1 };

        mockMapper.Setup(m => m.Map<Paginate<RecipeListItemResponse>>(It.IsAny<It.IsAnyType>())).Returns(paginatedDtos);
        mockMapper.Setup(m => m.Map(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<Type>())).Returns(paginatedDtos);
        mockMapper.Setup(m => m.Map<Paginate<RecipeListItemResponse>>(It.IsAny<Paginate<Recipe>>(), It.IsAny<Action<IMappingOperationOptions<object, Paginate<RecipeListItemResponse>>>>())).Returns(paginatedDtos);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }
}
