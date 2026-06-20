using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Queries.GetDeletedAdminRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetDeletedAdminRecipes;

public class GetDeletedAdminRecipesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnDeletedRecipes()
    {
        var mockRecipeRepo = new Mock<IRecipeRepository>();
        var mockUserRepo = new Mock<IUserRepository>();

        var recipes = new Paginate<Recipe> { Items = new List<Recipe>() };
        var users = new Paginate<User> { Items = new List<User>() };

        mockRecipeRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>())).ReturnsAsync(recipes);
        mockUserRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var handler = new GetDeletedAdminRecipesQueryHandler(mockRecipeRepo.Object, mockUserRepo.Object);

        var result = await handler.Handle(new GetDeletedAdminRecipesQuery(), CancellationToken.None);

        result.Should().NotBeNull();
    }
}
