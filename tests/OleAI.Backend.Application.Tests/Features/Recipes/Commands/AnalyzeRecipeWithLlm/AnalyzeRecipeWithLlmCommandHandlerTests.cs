using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.AnalyzeRecipeWithLlm;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.AnalyzeRecipeWithLlm;

public class AnalyzeRecipeWithLlmCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<ILocalLlmService> _mockLocalLlmService;
    private readonly AnalyzeRecipeWithLlmCommandHandler _handler;

    public AnalyzeRecipeWithLlmCommandHandlerTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockLocalLlmService = new Mock<ILocalLlmService>();
        _handler = new AnalyzeRecipeWithLlmCommandHandler(_mockRecipeRepository.Object, _mockLocalLlmService.Object);
    }

    [Fact]
    public async Task Handle_WhenRecipeExists_ShouldAnalyzeAndUpdate()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var command = new AnalyzeRecipeWithLlmCommand { RecipeId = recipeId };
        var recipe = Recipe.Create(Guid.NewGuid(), "Title", "Desc", RecipeDifficulty.Easy, 2, 10, 20, RecipeVisibility.Public);
        recipe.AddStep(1, "Step 1");

        _mockRecipeRepository
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _mockLocalLlmService
            .Setup(l => l.AnalyzeRecipeContentAsync(recipe.Description, It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"result\":\"ok\"}");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("Yapay zeka analizi başarıyla tamamlandı.");
        recipe.LlmReviewResultJson.Should().Be("{\"result\":\"ok\"}");
        _mockRecipeRepository.Verify(r => r.UpdateAsync(recipe, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new AnalyzeRecipeWithLlmCommand { RecipeId = Guid.NewGuid() };

        _mockRecipeRepository
            .Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Recipe, bool>>>(),
                It.IsAny<Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe)null!);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Tarif bulunamadı.");
    }
}
