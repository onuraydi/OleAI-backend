using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Users.Queries.GetAdminUsers;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Queries.GetAdminUsers;

public class GetAdminUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly GetAdminUsersQueryHandler _handler;

    public GetAdminUsersQueryHandlerTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _handler = new GetAdminUsersQueryHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedList()
    {
        var query = new GetAdminUsersQuery { PageNumber = 1, PageSize = 10 };
        var users = new Paginate<User>
        {
            Items = new List<User> { new User { Id = Guid.NewGuid(), FirstName = "First", LastName = "Last", Email = "email@email.com" } },
            PageNumber = 1,
            PageSize = 10,
            TotalItems = 1,
            TotalPages = 1
        };

        _mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }
}
