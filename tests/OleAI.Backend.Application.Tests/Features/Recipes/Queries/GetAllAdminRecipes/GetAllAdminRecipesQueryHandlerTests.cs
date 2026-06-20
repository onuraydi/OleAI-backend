using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Queries.GetAllAdminRecipes;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetAllAdminRecipes;

public class GetAllAdminRecipesQueryHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly GetAllAdminRecipesQueryHandler _handler;

    public GetAllAdminRecipesQueryHandlerTests()
    {
        _mockRecipeRepo = new Mock<IRecipeRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _handler = new GetAllAdminRecipesQueryHandler(_mockRecipeRepo.Object, _mockUserRepo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedList()
    {
        var query = new GetAllAdminRecipesQuery { Page = 0, Size = 10, Status = null, Visibility = null, SearchTerm = null };
        var recipes = new Paginate<Recipe>
        {
            Items = new List<Recipe> { Recipe.Create(System.Guid.NewGuid(), "Title", "Desc", RecipeDifficulty.Easy, 2, 10, 20, RecipeVisibility.Public) },
            PageNumber = 0,
            PageSize = 10,
            TotalItems = 1,
            TotalPages = 1
        };

        _mockRecipeRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var users = new Paginate<User>
        {
            Items = new List<User>()
        };
        _mockUserRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }
}
