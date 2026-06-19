using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.ContactMessages.Commands.MarkMessageAsRead;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;
using static OleAI.Backend.Application.Features.ContactMessages.Commands.MarkMessageAsRead.MarkContactMessageAsReadCommand;

namespace OleAI.Backend.Application.Tests.Features.ContactMessages.Commands.MarkMessageAsRead;

public class MarkContactMessageAsReadCommandHandlerTests
{
    private readonly Mock<IContactMessageRepository> _contactMessageRepositoryMock;
    private readonly MarkContactMessageAsReadCommandHandler _handler;

    public MarkContactMessageAsReadCommandHandlerTests()
    {
        _contactMessageRepositoryMock = new Mock<IContactMessageRepository>();

        _handler = new MarkContactMessageAsReadCommandHandler(_contactMessageRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenContactMessageExists_ShouldMarkAsReadAndReturnTrue()
    {
        // Arrange
        var command = new MarkContactMessageAsReadCommand { Id = Guid.NewGuid() };

        var contactMessage = new ContactMessage
        {
            Id = command.Id,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Department = "Support",
            Message = "Need help.",
            IsRead = false
        };

        _contactMessageRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<ContactMessage, bool>>>(),
                It.IsAny<Func<IQueryable<ContactMessage>, IIncludableQueryable<ContactMessage, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(contactMessage);

        _contactMessageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ContactMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contactMessage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        contactMessage.IsRead.Should().BeTrue();

        _contactMessageRepositoryMock.Verify(x => x.UpdateAsync(It.Is<ContactMessage>(c => c.Id == command.Id && c.IsRead == true), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenContactMessageDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new MarkContactMessageAsReadCommand { Id = Guid.NewGuid() };

        _contactMessageRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<ContactMessage, bool>>>(),
                It.IsAny<Func<IQueryable<ContactMessage>, IIncludableQueryable<ContactMessage, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContactMessage?)null);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Message not found.");

        _contactMessageRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ContactMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
