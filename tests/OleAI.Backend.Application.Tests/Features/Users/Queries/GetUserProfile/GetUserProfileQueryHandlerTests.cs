using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Features.Users.Profiles;
using OleAI.Backend.Application.Features.Users.Queries.GetUserProfile;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly UserBusinessRules _userBusinessRules;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetUserProfileQueryHandler _handler;

    public GetUserProfileQueryHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCacheService = new Mock<ICacheService>();

        _userBusinessRules = new UserBusinessRules(
            Mock.Of<IDietaryPreferenceRepository>(),
            Mock.Of<IAllergyRestrictionRepository>());

        _mockMapper = new Mock<IMapper>();

        _handler = new GetUserProfileQueryHandler(
            _mockUserRepo.Object,
            _mockMapper.Object,
            _mockHttpContextAccessor.Object,
            _mockCacheService.Object,
            _userBusinessRules);
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ShouldReturnCachedProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var cachedProfile = new UserProfileResponse
        {
            Id = userId,
            FirstName = "Cached",
            LastName = "User",
            Email = "cached@test.com"
        };

        _mockCacheService.Setup(c => c.GetAsync<UserProfileResponse>($"user-profile:{userId}"))
            .ReturnsAsync(cachedProfile);

        _mockMapper.Setup(m => m.Map<UserProfileResponse>(It.IsAny<User>()))
            .Returns(cachedProfile);

        // Act
        var result = await _handler.Handle(new GetUserProfileQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cachedProfile);
        _mockUserRepo.Verify(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
            It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
            It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ShouldFetchFromDbAndCacheResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockCacheService.Setup(c => c.GetAsync<UserProfileResponse>(It.IsAny<string>()))
            .ReturnsAsync((UserProfileResponse?)null);

        var user = new User
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            IsEmailVerified = true
        };

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var expectedResponse = new UserProfileResponse
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        };
        _mockMapper.Setup(m => m.Map<UserProfileResponse>(It.IsAny<User>())).Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(new GetUserProfileQuery(), CancellationToken.None);

        // Assert
        result.FirstName.Should().Be("Test");
        result.LastName.Should().Be("User");
        result.Email.Should().Be("test@test.com");
        _mockCacheService.Verify(c => c.SetAsync(
            $"user-profile:{userId}",
            It.IsAny<UserProfileResponse>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<TimeSpan?>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockCacheService.Setup(c => c.GetAsync<UserProfileResponse>(It.IsAny<string>()))
            .ReturnsAsync((UserProfileResponse?)null);

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _handler.Handle(new GetUserProfileQuery(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
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
