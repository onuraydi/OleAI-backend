using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile;
using OleAI.Backend.Application.Features.Users.Constants;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<OleAI.Backend.Application.Services.ImageStorage.IImageService> _mockImageService;
    private readonly UserBusinessRules _userBusinessRules;
    private readonly UpdateUserProfileCommandHandler _handler;

    public UpdateUserProfileCommandHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCacheService = new Mock<ICacheService>();
        _mockImageService = new Mock<OleAI.Backend.Application.Services.ImageStorage.IImageService>();

        _userBusinessRules = new UserBusinessRules(
            Mock.Of<IDietaryPreferenceRepository>(),
            Mock.Of<IAllergyRestrictionRepository>());

        _handler = new UpdateUserProfileCommandHandler(
            _mockUserRepo.Object,
            _mockHttpContextAccessor.Object,
            _mockCacheService.Object,
            _userBusinessRules,
            _mockImageService.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldUpdateAndInvalidateCache()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var user = new User { Id = userId, FirstName = "Old", LastName = "Name", Email = "e@e.com" };

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new UpdateUserProfileCommand { FirstName = "New", LastName = "Name" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(UserMessages.ProfileUpdatedSuccessfully);
        user.FirstName.Should().Be("New");
        user.LastName.Should().Be("Name");
        _mockUserRepo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(c => c.RemoveAsync($"user-profile:{userId}"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new UpdateUserProfileCommand { FirstName = "Test", LastName = "User" };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldTrimWhitespace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var user = new User { Id = userId, FirstName = "Old", LastName = "Name", Email = "e@e.com" };

        _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<User>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, object>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new UpdateUserProfileCommand { FirstName = "  Trimmed  ", LastName = "  Name  " };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.FirstName.Should().Be("Trimmed");
        user.LastName.Should().Be("Name");
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
