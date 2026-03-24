using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.ForgotPassword;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IMailService> _mockMailService;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _mockMailService = new Mock<IMailService>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new ForgotPasswordCommandHandler(
            _mockUserRepository.Object,
            _mockCacheService.Object,
            _mockMailService.Object,
            _authBusinessRules
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldSendOtpEmail()
    {
        // Arrange
        var request = new ForgotPasswordRequest { Email = "test@example.com" };
        var command = new ForgotPasswordCommand { Request = request };

        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

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
        result.Message.Should().Be(AuthMessages.ForgotPasswordSuccess);

        var redisKey = $"OTP:Reset:{request.Email}";
        _mockCacheService.Verify(c => c.SetAsync(redisKey, It.IsAny<string>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Once);
        _mockMailService.Verify(m => m.sendAsync(request.Email, AuthMessages.PasswordResetSubject, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowBusinessException()
    {
        // Arrange
        var command = new ForgotPasswordCommand
        {
            Request = new ForgotPasswordRequest { Email = "nobody@example.com" }
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
}
