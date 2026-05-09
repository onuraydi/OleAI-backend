using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Features.Users.Queries.GetUserProfile;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Queries;

public class GetUserProfileQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly UserBusinessRules _userBusinessRules;
    private readonly GetUserProfileQueryHandler _handler;

    public GetUserProfileQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCacheService = new Mock<ICacheService>();
        
        var mockDietaryRepo = new Mock<IDietaryPreferenceRepository>();
        var mockAllergyRepo = new Mock<IAllergyRestrictionRepository>();
        _userBusinessRules = new UserBusinessRules(mockDietaryRepo.Object, mockAllergyRepo.Object);

        _handler = new GetUserProfileQueryHandler(
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockHttpContextAccessor.Object,
            _mockCacheService.Object,
            _userBusinessRules);
    }

    [Fact]
    public async Task Handle_WhenProfileInCache_ShouldReturnCachedProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContext(userId);
        
        var cachedProfile = new UserProfileResponse { Id = userId };
        _mockCacheService.Setup(c => c.GetAsync<UserProfileResponse>($"user-profile:{userId}")).ReturnsAsync(cachedProfile);

        var query = new GetUserProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(cachedProfile);
        _mockUserRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<Func<IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProfileNotInCache_ShouldFetchFromDbAndCache()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContext(userId);
        
        _mockCacheService.Setup(c => c.GetAsync<UserProfileResponse>($"user-profile:{userId}")).ReturnsAsync((UserProfileResponse)null!);

        var user = new User { Id = userId };
        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), 
            It.IsAny<Func<IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>?>(), 
            false, 
            false,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var mappedProfile = new UserProfileResponse { Id = userId };
        _mockMapper.Setup(m => m.Map<UserProfileResponse>(user)).Returns(mappedProfile);

        var query = new GetUserProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(mappedProfile);
        _mockCacheService.Verify(c => c.SetAsync($"user-profile:{userId}", mappedProfile, It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Once);
    }

    private void SetupHttpContext(Guid userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
    }
}
