using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Commands.UpdateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommandHandlerTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly IngredientBusinessRules _rules;
    private readonly UpdateIngredientCommandHandler _handler;

    public UpdateIngredientCommandHandlerTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCacheService = new Mock<ICacheService>();

        _rules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _handler = new UpdateIngredientCommandHandler(_mockIngredientRepository.Object, _mockMapper.Object, _rules, _mockCacheService.Object);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateIngredientCommand
        {
            Request = new UpdateIngredientRequest { Id = Guid.NewGuid(), Name = "Yeni", CategoryId = Guid.NewGuid(), UnitFamily = MeasurementCategory.Weight }
        };

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
    public async Task Handle_WhenDuplicateNameExists_ShouldThrowBusinessException()
    {
        // Arrange
        var ingredientId = Guid.NewGuid();
        var existingIngredient = new Ingredient(ingredientId, "Eski", Guid.NewGuid(), MeasurementCategory.Weight);

        var command = new UpdateIngredientCommand
        {
            Request = new UpdateIngredientRequest { Id = ingredientId, Name = "Yeni", CategoryId = Guid.NewGuid(), UnitFamily = MeasurementCategory.Weight }
        };

        _mockIngredientRepository
            .SetupSequence(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingIngredient)
            .ReturnsAsync(new Ingredient(Guid.NewGuid(), "Yeni", Guid.NewGuid(), MeasurementCategory.Weight));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(IngredientMessages.ActiveIngredientAlreadyExistsMessage);

        _mockIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldUpdateIngredientAndReturnSuccessMessage()
    {
        // Arrange
        var ingredientId = Guid.NewGuid();
        var existingIngredient = new Ingredient(ingredientId, "Eski", Guid.NewGuid(), MeasurementCategory.Weight);

        var command = new UpdateIngredientCommand
        {
            Request = new UpdateIngredientRequest { Id = ingredientId, Name = "Yeni", CategoryId = Guid.NewGuid(), UnitFamily = MeasurementCategory.Count }
        };

        _mockIngredientRepository
            .SetupSequence(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingIngredient)
            .ReturnsAsync((Ingredient?)null);

        _mockIngredientRepository
            .Setup(r => r.UpdateAsync(existingIngredient, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingIngredient);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(IngredientMessages.IngredientUpdatedMessage);

        _mockMapper.Verify(m => m.Map(command.Request, existingIngredient), Times.Once);
        _mockIngredientRepository.Verify(r => r.UpdateAsync(existingIngredient, It.IsAny<CancellationToken>()), Times.Once);
    }
}
