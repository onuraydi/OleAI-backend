using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.ContactMessages.Commands.DeleteContactMessage;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ContactMessages.Commands.DeleteContactMessage;

public class DeleteContactMessageCommandHandlerTests
{
    private readonly Mock<IContactMessageRepository> _contactMessageRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DeleteContactMessageCommandHandler _handler;

    public DeleteContactMessageCommandHandlerTests()
    {
        _contactMessageRepositoryMock = new Mock<IContactMessageRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new DeleteContactMessageCommandHandler(
            _contactMessageRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenContactMessageExists_ShouldDeleteAndReturnResponse()
    {
        // Arrange
        var command = new DeleteContactMessageCommand { Id = Guid.NewGuid() };

        var contactMessage = new ContactMessage
        {
            Id = command.Id,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Department = "Support",
            Message = "Need help.",
            IsRead = false
        };

        var expectedResponse = new DeletedContactMessageResponse
        {
            Id = command.Id
        };

        _contactMessageRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<ContactMessage, bool>>>(),
                It.IsAny<Func<IQueryable<ContactMessage>, IIncludableQueryable<ContactMessage, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(contactMessage);

        _contactMessageRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<ContactMessage>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contactMessage);

        _mapperMock.Setup(x => x.Map<DeletedContactMessageResponse>(It.IsAny<ContactMessage>()))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedResponse.Id);

        _contactMessageRepositoryMock.Verify(x => x.DeleteAsync(It.Is<ContactMessage>(c => c.Id == command.Id), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenContactMessageDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteContactMessageCommand { Id = Guid.NewGuid() };

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
        await action.Should().ThrowAsync<Exception>().WithMessage("Contact Message not found");

        _contactMessageRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<ContactMessage>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
