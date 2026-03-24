using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.ResendOtp;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.ResendOtp;

public class ResendOtpCodeCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IMailService> _mockMailService;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly ResendOtpCodeCommandHandler _handler;

    public ResendOtpCodeCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _mockMailService = new Mock<IMailService>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new ResendOtpCodeCommandHandler(
            _mockUserRepository.Object,
            _authBusinessRules,
            _mockCacheService.Object,
            _mockMailService.Object
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldResendOtp()
    {
        // Arrange
        var request = new ResendOtpCodeRequest { Email = "test@example.com" };
        var command = new ResendOtpCommand { Request = request };

        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Status = false };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be(AuthMessages.ResendOtpCodeSuccessfully);

        var redisKey = $"OTP:Verify:{request.Email}";
        _mockCacheService.Verify(c => c.SetAsync(redisKey, It.IsAny<string>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Once);
        _mockMailService.Verify(m => m.sendAsync(request.Email, AuthMessages.EmailVerificationSubject, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowBusinessException()
    {
        // Arrange
        var command = new ResendOtpCommand
        {
            Request = new ResendOtpCodeRequest { Email = "nobody@example.com" }
        };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.UserNotFound);
    }

    /// <summary>
    /// NOTE: The source handler (ResendOtpCodeCommandHandler.cs line 31) calls
    /// _authBusinessRules.UserAlreadyVerified(user.Email) WITHOUT await.
    /// The returned Task is fire-and-forget — the BusinessException is never propagated.
    /// This test documents the current (buggy) behavior.
    /// TODO: Fix the handler to await UserAlreadyVerified so the guard is actually enforced.
    /// </summary>
    [Fact]
    public async Task Handle_UserAlreadyVerified_CurrentBehavior_ShouldNotThrow_DueToUnawaited_Bug()
    {
        // Arrange
        var email = "verified@example.com";
        var command = new ResendOtpCommand
        {
            Request = new ResendOtpCodeRequest { Email = email }
        };

        var verifiedUser = new User { Id = Guid.NewGuid(), Email = email, Status = true };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(verifiedUser);

        // Act & Assert – exception is swallowed due to missing await
        // When the source bug is fixed, change to ThrowAsync<BusinessException>
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().NotThrowAsync();
    }
}
