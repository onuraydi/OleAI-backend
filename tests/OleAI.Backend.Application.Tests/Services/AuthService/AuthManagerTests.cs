using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Services.AuthService;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Core.Security.Jwt;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Services.AuthService;

public class AuthManagerTests
{
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private readonly AuthManager _authManager;

    public AuthManagerTests()
    {
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();

        var tokenOptions = new TokenOptions { MaxActiveTokens = 2 };

        _authManager = new AuthManager(_mockRefreshTokenRepo.Object, tokenOptions);
    }

    [Fact]
    public async Task DeleteOldRefreshTokensAsync_ShouldExecuteDeleteForInvalidTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Active tokens setup (empty so it won't trigger the secondary delete limit logic)
        _mockRefreshTokenRepo.Setup(r => r.GetListAsync(
            It.IsAny<QueryOptions<Domain.Entities.RefreshToken>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<Domain.Entities.RefreshToken> { Items = new List<Domain.Entities.RefreshToken>() });

        // Act
        await _authManager.DeleteOldRefreshTokensAsync(userId, CancellationToken.None);

        // Assert
        _mockRefreshTokenRepo.Verify(r => r.ExecuteDeleteAsync(
            It.IsAny<Expression<Func<Domain.Entities.RefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOldRefreshTokensAsync_ShouldLimitActiveTokens_WhenCountExceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // 3 active tokens. Limit is 2. So we keep the most recent (limit - 1) = 1.
        // The remaining 2 should be deleted.
        var activeTokens = new List<Domain.Entities.RefreshToken>
        {
            new Domain.Entities.RefreshToken { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow }, // Most recent
            new Domain.Entities.RefreshToken { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow.AddSeconds(-10) }, // Oldest 1
            new Domain.Entities.RefreshToken { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow.AddSeconds(-20) }  // Oldest 2
        };

        var paginated = new Paginate<Domain.Entities.RefreshToken> { Items = activeTokens };

        _mockRefreshTokenRepo.Setup(r => r.GetListAsync(
            It.IsAny<QueryOptions<Domain.Entities.RefreshToken>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginated);

        // Act
        await _authManager.DeleteOldRefreshTokensAsync(userId, CancellationToken.None);

        // Assert
        _mockRefreshTokenRepo.Verify(r => r.ExecuteDeleteAsync(
            It.IsAny<Expression<Func<Domain.Entities.RefreshToken, bool>>>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockRefreshTokenRepo.Verify(r => r.DeleteRangeAsync(
            It.Is<IEnumerable<Domain.Entities.RefreshToken>>(list => list.Count() == 2),
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
