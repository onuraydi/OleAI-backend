using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.Register;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IMailService> _mockMailService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _mockMailService = new Mock<IMailService>();
        _mockMapper = new Mock<IMapper>();
        _authBusinessRules = new AuthBusinessRules(_mockUserRepository.Object);

        _handler = new RegisterCommandHandler(
            _mockUserRepository.Object,
            _authBusinessRules,
            _mockCacheService.Object,
            _mockMailService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldRegisterUserAndSendOtp()
    {
        // Arrange
        var request = new RegisterRequest { Email = "test@example.com", Password = "password123", FirstName = "Test", LastName = "User" };
        var command = new RegisterCommand { Request = request };

        _mockUserRepository.Setup(r => r.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
            false,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null); // No existing user

        _mockMapper.Setup(m => m.Map<User>(It.IsAny<RegisterRequest>()))
            .Returns(new User { Email = request.Email, FirstName = request.FirstName, LastName = request.LastName });

        _mockMapper.Setup(m => m.Map<RegisteredResponse>(It.IsAny<User>()))
            .Returns(new RegisteredResponse { Id = Guid.NewGuid(), Email = request.Email });

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be(AuthMessages.SuccessfulRegistration);

        // Ensure AddAsync is called with the generated hash, salt, and status = false
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == request.Email &&
            u.PasswordHash != null &&
            u.PasswordSalt != null &&
            u.Status == false), It.IsAny<CancellationToken>()), Times.Once);

        // Ensure Cache is set
        _mockCacheService.Verify(c => c.SetAsync(It.Is<string>(k => k == $"OTP:Verify:{request.Email}"), It.IsAny<string>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Once);

        // Ensure mail is sent
        _mockMailService.Verify(m => m.sendAsync(request.Email, AuthMessages.EmailVerificationSubject, It.IsAny<string>()), Times.Once);
    }
}
