using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;
using OleAI.Backend.Application.Services.Repositories;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Commands.Tag;

public class UpdateTagCommandHandlerTests
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _tagRepositoryMock = new Mock<ITagRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new UpdateTagCommandHandler(
            _tagRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldUpdateAndReturnSuccessMessage()
    {
        // Arrange
        var command = new UpdateTagCommand { Id = 1, Name = "Updated Vegan" };
        var tag = new OleAI.Backend.Domain.Entities.Tag(1, "Vegan");

        _tagRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.Tag, bool>>>(),
                It.IsAny<Func<IQueryable<OleAI.Backend.Domain.Entities.Tag>, IIncludableQueryable<OleAI.Backend.Domain.Entities.Tag, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _mapperMock.Setup(x => x.Map(command, tag)).Callback<UpdateTagCommand, OleAI.Backend.Domain.Entities.Tag>((src, dest) =>
        {
        });

        _tagRepositoryMock.Setup(x => x.UpdateAsync(tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Etiket başarıyla güncellendi.");
        // In tests we can't test mapped properties directly because they are private set and auto-mapper does the reflection.
        // We just verify mapping and update were called.

        _tagRepositoryMock.Verify(x => x.UpdateAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map(command, tag), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateTagCommand { Id = 1, Name = "Updated Vegan" };

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

        _tagRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OleAI.Backend.Domain.Entities.Tag>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
