using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Commands.DeleteIngredient;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Commands.DeleteIngredient;

public class DeleteIngredientCommandHandlerTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly IngredientBusinessRules _rules;
    private readonly DeleteIngredientCommandHandler _handler;

    public DeleteIngredientCommandHandlerTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockCacheService = new Mock<ICacheService>();

        _rules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _handler = new DeleteIngredientCommandHandler(_mockIngredientRepository.Object, _mockUserIngredientRepository.Object, _rules, _mockCacheService.Object);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new DeleteIngredientCommand { Id = Guid.NewGuid() };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(IngredientMessages.IngredientNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenIngredientUsedInInventory_ShouldSoftDeleteAndReturnSoftDeleteMessage()
    {
        // Arrange
        var ingredient = new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), MeasurementCategory.Weight);
        var command = new DeleteIngredientCommand { Id = ingredient.Id };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        _mockUserIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<UserIngredient>
            {
                Items = new List<UserIngredient>
                {
                    UserIngredient.CreateStandard(Guid.NewGuid(), Guid.NewGuid(), ingredient.Id, 2, MeasurementUnit.Gram)
                }
            });

        _mockIngredientRepository
            .Setup(r => r.DeleteAsync(ingredient, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(IngredientMessages.IngredientSoftDeletedDueToInventoryUsageMessage);
        _mockIngredientRepository.Verify(r => r.DeleteAsync(ingredient, false, It.IsAny<CancellationToken>()), Times.Once);
        _mockIngredientRepository.Verify(r => r.DeleteAsync(ingredient, true, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotUsedInInventory_ShouldHardDeleteAndReturnDeletedMessage()
    {
        // Arrange
        var ingredient = new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), MeasurementCategory.Weight);
        var command = new DeleteIngredientCommand { Id = ingredient.Id };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        _mockUserIngredientRepository
            .Setup(r => r.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Paginate<UserIngredient> { Items = new List<UserIngredient>() });

        _mockIngredientRepository
            .Setup(r => r.DeleteAsync(ingredient, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(IngredientMessages.IngredientDeletedMessage);
        _mockIngredientRepository.Verify(r => r.DeleteAsync(ingredient, true, It.IsAny<CancellationToken>()), Times.Once);
        _mockIngredientRepository.Verify(r => r.DeleteAsync(ingredient, false, It.IsAny<CancellationToken>()), Times.Never);
    }
}
