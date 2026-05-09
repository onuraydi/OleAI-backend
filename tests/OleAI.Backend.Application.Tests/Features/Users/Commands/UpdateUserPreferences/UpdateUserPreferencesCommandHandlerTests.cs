using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences;
using OleAI.Backend.Application.Features.Users.Constants;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Commands.UpdateUserPreferences;

public class UpdateUserPreferencesCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IUserDietaryPreferenceRepository> _mockPrefRepo;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IDietaryPreferenceRepository> _mockDietaryPrefRepo;
    private readonly Mock<IUserProfileVectorService> _mockVectorService;
    private readonly UpdateUserPreferencesCommandHandler _handler;

    public UpdateUserPreferencesCommandHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockPrefRepo = new Mock<IUserDietaryPreferenceRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCacheService = new Mock<ICacheService>();
        _mockDietaryPrefRepo = new Mock<IDietaryPreferenceRepository>();
        _mockVectorService = new Mock<IUserProfileVectorService>();

        var rules = new UserBusinessRules(
            _mockDietaryPrefRepo.Object,
            Mock.Of<IAllergyRestrictionRepository>());

        _handler = new UpdateUserPreferencesCommandHandler(
            _mockUserRepo.Object,
            _mockPrefRepo.Object,
            _mockHttpContextAccessor.Object,
            _mockCacheService.Object,
            rules,
            _mockVectorService.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldUpsertAndInvalidateCache()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var prefId1 = Guid.NewGuid();
        var prefId2 = Guid.NewGuid();

        _mockUserRepo.Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockDietaryPrefRepo.Setup(r => r.GetCountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<DietaryPreference, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var command = new UpdateUserPreferencesCommand { DietaryPreferenceIds = new List<Guid> { prefId1, prefId2 } };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(UserMessages.DietaryPreferencesUpdatedSuccessfully);
        _mockPrefRepo.Verify(r => r.ExecuteDeleteByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockPrefRepo.Verify(r => r.AddRangeAsync(
            It.Is<IEnumerable<UserDietaryPreference>>(list => list.Count() == 2),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(c => c.RemoveAsync($"user-profile:{userId}"), Times.Once);
        _mockVectorService.Verify(v => v.UpdateDietaryProfileAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockUserRepo.Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateUserPreferencesCommand { DietaryPreferenceIds = new List<Guid> { Guid.NewGuid() } };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenInvalidPreferenceIds_ShouldThrowBusinessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockUserRepo.Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Sadece 1 tanesi DB'de var, ama 2 tane gönderildi
        _mockDietaryPrefRepo.Setup(r => r.GetCountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<DietaryPreference, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateUserPreferencesCommand { DietaryPreferenceIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() } };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task Handle_WhenEmptyList_ShouldClearAllPreferences()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockUserRepo.Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UpdateUserPreferencesCommand { DietaryPreferenceIds = new List<Guid>() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(UserMessages.DietaryPreferencesUpdatedSuccessfully);
        _mockPrefRepo.Verify(r => r.ExecuteDeleteByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockPrefRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<UserDietaryPreference>>(), It.IsAny<CancellationToken>()), Times.Never);
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
