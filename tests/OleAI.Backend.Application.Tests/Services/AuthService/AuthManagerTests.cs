using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Services.AuthService;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Core.Security.Jwt;
using Xunit;
using DomainEntities = OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Tests.Services.AuthService;

public class AuthManagerTests
{
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
    private readonly TokenOptions _tokenOptions;
    private readonly AuthManager _authManager;

    public AuthManagerTests()
    {
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _tokenOptions = new TokenOptions { MaxActiveTokens = 2 };
        _authManager = new AuthManager(_mockRefreshTokenRepository.Object, _tokenOptions);
    }

    [Fact]
    public async Task DeleteOldRefreshTokensAsync_WhenCalled_ShouldDeleteInvalidTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockRefreshTokenRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<DomainEntities.RefreshToken>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<DomainEntities.RefreshToken> { Items = new List<DomainEntities.RefreshToken>() });

        // Act
        await _authManager.DeleteOldRefreshTokensAsync(userId, CancellationToken.None);

        // Assert
        _mockRefreshTokenRepository.Verify(r => r.ExecuteDeleteAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DomainEntities.RefreshToken, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOldRefreshTokensAsync_WhenMaxTokensExceeded_ShouldDeleteOldest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokens = new List<DomainEntities.RefreshToken>
        {
            new() { UserId = userId, CreatedDate = DateTime.UtcNow },
            new() { UserId = userId, CreatedDate = DateTime.UtcNow.AddMinutes(-1) },
            new() { UserId = userId, CreatedDate = DateTime.UtcNow.AddMinutes(-2) }
        };

        _mockRefreshTokenRepository.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<DomainEntities.RefreshToken>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<DomainEntities.RefreshToken> { Items = tokens });

        // Act
        await _authManager.DeleteOldRefreshTokensAsync(userId, CancellationToken.None);

        // Assert
        _mockRefreshTokenRepository.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<DomainEntities.RefreshToken>>(), true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
