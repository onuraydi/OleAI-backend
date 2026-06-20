using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;
using OleAI.Backend.Application.Services.Repositories;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Commands.Tag;

public class CreateTagCommandHandlerTests
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTagCommandHandler _handler;

    public CreateTagCommandHandlerTests()
    {
        _tagRepositoryMock = new Mock<ITagRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateTagCommandHandler(
            _tagRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTagAndReturnSuccessMessage()
    {
        // Arrange
        var command = new CreateTagCommand { Name = "Vegan" };
        var tag = new OleAI.Backend.Domain.Entities.Tag(1, "Vegan");

        _mapperMock.Setup(x => x.Map<OleAI.Backend.Domain.Entities.Tag>(command)).Returns(tag);
        _tagRepositoryMock.Setup(x => x.AddAsync(tag, It.IsAny<CancellationToken>())).ReturnsAsync(tag);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Etiket başarıyla eklendi.");

        _tagRepositoryMock.Verify(x => x.AddAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map<OleAI.Backend.Domain.Entities.Tag>(command), Times.Once);
    }
}
