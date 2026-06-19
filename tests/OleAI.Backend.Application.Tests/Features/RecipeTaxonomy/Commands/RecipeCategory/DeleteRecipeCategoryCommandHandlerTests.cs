using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;
using OleAI.Backend.Application.Services.Repositories;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class DeleteRecipeCategoryCommandHandlerTests
{
    private readonly Mock<IRecipeCategoryRepository> _recipeCategoryRepositoryMock;
    private readonly DeleteRecipeCategoryCommandHandler _handler;

    public DeleteRecipeCategoryCommandHandlerTests()
    {
        _recipeCategoryRepositoryMock = new Mock<IRecipeCategoryRepository>();

        _handler = new DeleteRecipeCategoryCommandHandler(_recipeCategoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldDeleteAndReturnSuccessMessage()
    {
        // Arrange
        var command = new DeleteRecipeCategoryCommand { Id = 1 };
        var category = new OleAI.Backend.Domain.Entities.RecipeCategory(1, "Desserts");

        _recipeCategoryRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.RecipeCategory, bool>>>(),
                It.IsAny<Func<IQueryable<OleAI.Backend.Domain.Entities.RecipeCategory>, IIncludableQueryable<OleAI.Backend.Domain.Entities.RecipeCategory, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _recipeCategoryRepositoryMock.Setup(x => x.DeleteAsync(category, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Tarif kategorisi başarıyla silindi.");

        _recipeCategoryRepositoryMock.Verify(x => x.DeleteAsync(category, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteRecipeCategoryCommand { Id = 1 };

        _recipeCategoryRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.RecipeCategory, bool>>>(),
                It.IsAny<Func<IQueryable<OleAI.Backend.Domain.Entities.RecipeCategory>, IIncludableQueryable<OleAI.Backend.Domain.Entities.RecipeCategory, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((OleAI.Backend.Domain.Entities.RecipeCategory?)null);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Tarif kategorisi bulunamadı.");

        _recipeCategoryRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<OleAI.Backend.Domain.Entities.RecipeCategory>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
