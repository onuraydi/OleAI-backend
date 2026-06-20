using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;
using static OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage.CreateContactMessageCommand;

namespace OleAI.Backend.Application.Tests.Features.ContactMessages.Commands.CreateContactMessage;

public class CreateContactMessageCommandHandlerTests
{
    private readonly Mock<IContactMessageRepository> _contactMessageRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateContactMessageCommandHandler _handler;

    public CreateContactMessageCommandHandlerTests()
    {
        _contactMessageRepositoryMock = new Mock<IContactMessageRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateContactMessageCommandHandler(
            _contactMessageRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateContactMessageAndReturnResponse()
    {
        // Arrange
        var command = new CreateContactMessageCommand
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Department = "Support",
            Message = "Need help with my account."
        };

        var createdMessageId = Guid.NewGuid();
        var createdMessage = new ContactMessage
        {
            Id = createdMessageId,
            Name = command.Name,
            Email = command.Email,
            Department = command.Department,
            Message = command.Message,
            IsRead = false
        };

        var expectedResponse = new CreateContactMessageResponse
        {
            Id = createdMessageId,
            Name = command.Name,
            Email = command.Email,
            Department = command.Department,
            Message = command.Message,
            IsRead = false
        };

        _contactMessageRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ContactMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdMessage);

        _mapperMock.Setup(x => x.Map<CreateContactMessageResponse>(It.IsAny<ContactMessage>()))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedResponse.Id);
        result.Name.Should().Be(expectedResponse.Name);
        result.Email.Should().Be(expectedResponse.Email);
        result.Department.Should().Be(expectedResponse.Department);
        result.Message.Should().Be(expectedResponse.Message);
        result.IsRead.Should().BeFalse();

        _contactMessageRepositoryMock.Verify(x => x.AddAsync(It.Is<ContactMessage>(cm =>
            cm.Name == command.Name &&
            cm.Email == command.Email &&
            cm.Department == command.Department &&
            cm.Message == command.Message &&
            cm.IsRead == false), It.IsAny<CancellationToken>()), Times.Once);
    }
}
