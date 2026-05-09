using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies;
using OleAI.Backend.Application.Features.Users.Constants;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Application.Services.UserProfiles;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Users.Commands.UpdateUserAllergies;

public class UpdateUserAllergiesCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IUserAllergyRepository> _mockAllergyRepo;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IAllergyRestrictionRepository> _mockAllergyRestrictionRepo;
    private readonly Mock<IUserProfileVectorService> _mockVectorService;
    private readonly UpdateUserAllergiesCommandHandler _handler;

    public UpdateUserAllergiesCommandHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockAllergyRepo = new Mock<IUserAllergyRepository>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCacheService = new Mock<ICacheService>();
        _mockAllergyRestrictionRepo = new Mock<IAllergyRestrictionRepository>();
        _mockVectorService = new Mock<IUserProfileVectorService>();

        var rules = new UserBusinessRules(
            Mock.Of<IDietaryPreferenceRepository>(),
            _mockAllergyRestrictionRepo.Object);

        _handler = new UpdateUserAllergiesCommandHandler(
            _mockUserRepo.Object,
            _mockAllergyRepo.Object,
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

        var allergyId1 = Guid.NewGuid();
        var allergyId2 = Guid.NewGuid();

        _mockUserRepo.Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockAllergyRestrictionRepo.Setup(r => r.GetCountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<AllergyRestriction, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var command = new UpdateUserAllergiesCommand { AllergyRestrictionIds = new List<Guid> { allergyId1, allergyId2 } };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(UserMessages.AllergiesUpdatedSuccessfully);
        _mockAllergyRepo.Verify(r => r.ExecuteDeleteByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockAllergyRepo.Verify(r => r.AddRangeAsync(
            It.Is<IEnumerable<UserAllergy>>(list => list.Count() == 2),
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

        var command = new UpdateUserAllergiesCommand { AllergyRestrictionIds = new List<Guid> { Guid.NewGuid() } };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenInvalidAllergyIds_ShouldThrowBusinessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockUserRepo.Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockAllergyRestrictionRepo.Setup(r => r.GetCountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<AllergyRestriction, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new UpdateUserAllergiesCommand { AllergyRestrictionIds = new List<Guid> { Guid.NewGuid() } };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task Handle_WhenEmptyList_ShouldClearAllAllergies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUser(userId);

        _mockUserRepo.Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UpdateUserAllergiesCommand { AllergyRestrictionIds = new List<Guid>() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(UserMessages.AllergiesUpdatedSuccessfully);
        _mockAllergyRepo.Verify(r => r.ExecuteDeleteByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockAllergyRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<UserAllergy>>(), It.IsAny<CancellationToken>()), Times.Never);
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
