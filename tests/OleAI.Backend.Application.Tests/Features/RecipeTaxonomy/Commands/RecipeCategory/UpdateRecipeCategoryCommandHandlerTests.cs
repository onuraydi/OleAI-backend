using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;
using OleAI.Backend.Application.Services.Repositories;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class UpdateRecipeCategoryCommandHandlerTests
{
    private readonly Mock<IRecipeCategoryRepository> _recipeCategoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateRecipeCategoryCommandHandler _handler;

    public UpdateRecipeCategoryCommandHandlerTests()
    {
        _recipeCategoryRepositoryMock = new Mock<IRecipeCategoryRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new UpdateRecipeCategoryCommandHandler(
            _recipeCategoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldUpdateAndReturnSuccessMessage()
    {
        // Arrange
        var command = new UpdateRecipeCategoryCommand { Id = 1, Name = "Updated Desserts", IconName = "updated-icon" };
        var category = new OleAI.Backend.Domain.Entities.RecipeCategory(1, "Desserts", "icon-dessert");

        _recipeCategoryRepositoryMock.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<OleAI.Backend.Domain.Entities.RecipeCategory, bool>>>(),
                It.IsAny<Func<IQueryable<OleAI.Backend.Domain.Entities.RecipeCategory>, IIncludableQueryable<OleAI.Backend.Domain.Entities.RecipeCategory, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mapperMock.Setup(x => x.Map(command, category)).Callback<UpdateRecipeCategoryCommand, OleAI.Backend.Domain.Entities.RecipeCategory>((src, dest) =>
        {
        });

        _recipeCategoryRepositoryMock.Setup(x => x.UpdateAsync(category, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Tarif kategorisi başarıyla güncellendi.");
        // In tests we can't test mapped properties directly because they are private set and auto-mapper does the reflection.
        // We just verify mapping and update were called.

        _recipeCategoryRepositoryMock.Verify(x => x.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map(command, category), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateRecipeCategoryCommand { Id = 1, Name = "Updated Desserts", IconName = "updated-icon" };

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

        _recipeCategoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OleAI.Backend.Domain.Entities.RecipeCategory>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
