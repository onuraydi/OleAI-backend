using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientById;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Queries.GetIngredientById;

public class GetIngredientByIdQueryHandlerTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IngredientBusinessRules _rules;
    private readonly GetIngredientByIdQueryHandler _handler;

    public GetIngredientByIdQueryHandlerTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();
        _rules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _handler = new GetIngredientByIdQueryHandler(_mockIngredientRepository.Object, _mockMapper.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenIngredientNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetIngredientByIdQuery { Id = Guid.NewGuid() };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(IngredientMessages.IngredientNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenIngredientExists_ShouldMapAndReturnResponse()
    {
        // Arrange
        var ingredient = new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), MeasurementCategory.Weight);
        var query = new GetIngredientByIdQuery { Id = ingredient.Id };

        var mappedResponse = new IngredientResponse
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            UnitFamily = ingredient.UnitFamily,
            CategoryName = "Baharat"
        };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        _mockMapper
            .Setup(m => m.Map<IngredientResponse>(ingredient))
            .Returns(mappedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ingredient.Id);
        result.Name.Should().Be("Tuz");

        _mockMapper.Verify(m => m.Map<IngredientResponse>(ingredient), Times.Once);
    }
}
