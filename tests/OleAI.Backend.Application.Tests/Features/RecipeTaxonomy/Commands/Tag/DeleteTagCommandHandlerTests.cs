using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;
using OleAI.Backend.Application.Services.Repositories;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Commands.Tag;

public class DeleteTagCommandHandlerTests
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly DeleteTagCommandHandler _handler;

    public DeleteTagCommandHandlerTests()
    {
        _tagRepositoryMock = new Mock<ITagRepository>();

        _handler = new DeleteTagCommandHandler(_tagRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldDeleteAndReturnSuccessMessage()
    {
        // Arrange
        var command = new DeleteTagCommand { Id = 1 };
        var tag = new OleAI.Backend.Domain.Entities.Tag(1, "Vegan");

        _tagRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.Tag, bool>>>(),
                It.IsAny<Func<IQueryable<OleAI.Backend.Domain.Entities.Tag>, IIncludableQueryable<OleAI.Backend.Domain.Entities.Tag, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _tagRepositoryMock.Setup(x => x.DeleteAsync(tag, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Etiket başarıyla silindi.");

        _tagRepositoryMock.Verify(x => x.DeleteAsync(tag, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteTagCommand { Id = 1 };

        _tagRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.Tag, bool>>>(),
                It.IsAny<Func<IQueryable<OleAI.Backend.Domain.Entities.Tag>, IIncludableQueryable<OleAI.Backend.Domain.Entities.Tag, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((OleAI.Backend.Domain.Entities.Tag?)null);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Etiket bulunamadı.");

        _tagRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<OleAI.Backend.Domain.Entities.Tag>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
