using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.RestoreCategory;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands.RestoreCategory;

public class RestoreCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRestoreCategory()
    {
        var mockRepo = new Mock<ICategoryRepository>();
        var handler = new RestoreCategoryCommandHandler(mockRepo.Object);

        var id = Guid.NewGuid();
        var command = new RestoreCategoryCommand { Id = id };
        var category = new Category(id, "N", "D") { DeletedDate = DateTime.UtcNow };

        mockRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, true, true, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        category.DeletedDate.Should().BeNull();
        mockRepo.Verify(r => r.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }
}
