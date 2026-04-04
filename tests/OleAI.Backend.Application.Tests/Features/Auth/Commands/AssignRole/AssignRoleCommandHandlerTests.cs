using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Auth.Commands.AssignRole;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Commands.AssignRole
{
    public class AssignRoleCommandHandlerTests
    {
        private readonly Mock<IUserOperationClaimRepository> _mockUserOpClaimRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IOperationClaimRepository> _mockOpClaimRepo;
        private readonly RoleBusinessRules _roleBusinessRules;
        private readonly AssignRoleCommandHandler _handler;

        public AssignRoleCommandHandlerTests()
        {
            _mockUserOpClaimRepo = new Mock<IUserOperationClaimRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockOpClaimRepo = new Mock<IOperationClaimRepository>();

            _roleBusinessRules = new RoleBusinessRules(
                _mockUserRepo.Object,
                _mockOpClaimRepo.Object,
                _mockUserOpClaimRepo.Object);

            _handler = new AssignRoleCommandHandler(
                _mockUserOpClaimRepo.Object,
                _roleBusinessRules);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ShouldThrowBusinessException()
        {
            // Arrange
            var command = new AssignRoleCommand
            {
                Request = new AssignRoleRequest { UserId = Guid.NewGuid(), OperationClaimId = Guid.NewGuid() }
            };

            _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(AuthMessages.TargetUserNotFound);
        }

        [Fact]
        public async Task Handle_OperationClaimDoesNotExist_ShouldThrowBusinessException()
        {
            // Arrange
            var command = new AssignRoleCommand
            {
                Request = new AssignRoleRequest { UserId = Guid.NewGuid(), OperationClaimId = Guid.NewGuid() }
            };

            _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User());

            _mockOpClaimRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<OperationClaim, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OperationClaim?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(AuthMessages.OperationClaimNotFound);
        }

        [Fact]
        public async Task Handle_UserAlreadyHasRole_ShouldThrowBusinessException()
        {
            // Arrange
            var command = new AssignRoleCommand
            {
                Request = new AssignRoleRequest { UserId = Guid.NewGuid(), OperationClaimId = Guid.NewGuid() }
            };

            _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User());

            _mockOpClaimRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<OperationClaim, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OperationClaim());

            _mockUserOpClaimRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<UserOperationClaim, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserOperationClaim()); // Returns existing claim

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(AuthMessages.UserAlreadyHasRole);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldAssignRoleAndReturnResponse()
        {
            // Arrange
            var command = new AssignRoleCommand
            {
                Request = new AssignRoleRequest { UserId = Guid.NewGuid(), OperationClaimId = Guid.NewGuid() }
            };

            _mockUserRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User());

            _mockOpClaimRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<OperationClaim, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OperationClaim());

            _mockUserOpClaimRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<UserOperationClaim, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserOperationClaim?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be(AuthMessages.RoleAssignedSuccessfully);

            _mockUserOpClaimRepo.Verify(r => r.AddAsync(
                It.Is<UserOperationClaim>(uc => uc.UserId == command.Request.UserId && uc.OperationClaimId == command.Request.OperationClaimId),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
