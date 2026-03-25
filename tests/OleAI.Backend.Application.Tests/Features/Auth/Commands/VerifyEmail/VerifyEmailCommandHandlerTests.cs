using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.VerifyEmail;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly VerifyEmailCommandHandler _handler;

    public VerifyEmailCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _mockMapper = new Mock<IMapper>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new VerifyEmailCommandHandler(
            _mockUserRepository.Object,
            _mockCacheService.Object,
            _authBusinessRules,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_ValidOtp_ShouldVerifyUserEmail()
    {
        // Arrange
        var request = new VerifyEmailRequest { Email = "test@example.com", OtpCode = "123456" };
        var command = new VerifyEmailCommand { Request = request };

        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Status = false };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockCacheService.Setup(c => c.GetAsync<string>($"OTP:Verify:{request.Email}"))
            .ReturnsAsync("123456");

        _mockMapper.Setup(m => m.Map<verifiedEmailResponse>(It.IsAny<User>()))
            .Returns(new verifiedEmailResponse { Email = request.Email });

        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be(AuthMessages.UserVerifiedMessage);

        user.Status.Should().BeTrue(); // Email verified

        _mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(c => c.RemoveAsync($"OTP:Verify:{request.Email}"), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowBusinessException()
    {
        // Arrange
        var command = new VerifyEmailCommand
        {
            Request = new VerifyEmailRequest { Email = "nobody@example.com", OtpCode = "123456" }
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

    [Fact]
    public async Task Handle_AlreadyVerifiedUser_ShouldThrowBusinessException()
    {
        // Arrange
        var email = "verified@example.com";
        var command = new VerifyEmailCommand
        {
            Request = new VerifyEmailRequest { Email = email, OtpCode = "123456" }
        };

        var verifiedUser = new User { Id = Guid.NewGuid(), Email = email, Status = true };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(verifiedUser);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.UserAlreadyVerified);
    }

    [Fact]
    public async Task Handle_InvalidOtpCode_ShouldThrowBusinessException()
    {
        // Arrange
        var email = "test@example.com";
        var command = new VerifyEmailCommand
        {
            Request = new VerifyEmailRequest { Email = email, OtpCode = "999999" }
        };

        var user = new User { Id = Guid.NewGuid(), Email = email, Status = false };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Cached code differs from provided OTP
        _mockCacheService.Setup(c => c.GetAsync<string>($"OTP:Verify:{email}"))
            .ReturnsAsync("123456");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(AuthMessages.VerificationCodeIsIncorrectOrExpired);
    }
}
