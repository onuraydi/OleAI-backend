using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class CreateRecipeCategoryCommandHandlerTests
{
    private readonly Mock<IRecipeCategoryRepository> _recipeCategoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateRecipeCategoryCommandHandler _handler;

    public CreateRecipeCategoryCommandHandlerTests()
    {
        _recipeCategoryRepositoryMock = new Mock<IRecipeCategoryRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateRecipeCategoryCommandHandler(
            _recipeCategoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateRecipeCategoryAndReturnSuccessMessage()
    {
        // Arrange
        var command = new CreateRecipeCategoryCommand { Name = "Desserts", IconName = "icon-dessert" };
        var category = new OleAI.Backend.Domain.Entities.RecipeCategory(1, "Desserts", "icon-dessert");

        _mapperMock.Setup(x => x.Map<OleAI.Backend.Domain.Entities.RecipeCategory>(command)).Returns(category);
        _recipeCategoryRepositoryMock.Setup(x => x.AddAsync(category, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Tarif kategorisi başarıyla eklendi.");

        _recipeCategoryRepositoryMock.Verify(x => x.AddAsync(category, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map<OleAI.Backend.Domain.Entities.RecipeCategory>(command), Times.Once);
    }
}
