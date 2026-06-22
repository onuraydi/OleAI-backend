using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Commands.CreateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Commands.CreateIngredient;

public class CreateIngredientCommandHandlerTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly IngredientBusinessRules _rules;
    private readonly CreateIngredientCommandHandler _handler;

    public CreateIngredientCommandHandlerTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCacheService = new Mock<ICacheService>();

        _rules = new IngredientBusinessRules(_mockIngredientRepository.Object);
        _handler = new CreateIngredientCommandHandler(_mockIngredientRepository.Object, _mockMapper.Object, _rules, _mockCacheService.Object);
    }

    [Fact]
    public async Task Handle_WhenDuplicateIngredientExists_ShouldThrowBusinessException()
    {
        // Arrange
        var command = new CreateIngredientCommand
        {
            Request = new CreateIngredientRequest { Name = "Tuz", CategoryId = Guid.NewGuid(), UnitFamily = MeasurementCategory.Weight }
        };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ingredient(Guid.NewGuid(), "Tuz", Guid.NewGuid(), MeasurementCategory.Weight));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(IngredientMessages.ActiveIngredientAlreadyExistsMessage);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCreateIngredientAndReturnResponse()
    {
        // Arrange
        var command = new CreateIngredientCommand
        {
            Request = new CreateIngredientRequest { Name = "Tuz", CategoryId = Guid.NewGuid(), UnitFamily = MeasurementCategory.Weight }
        };

        var mappedIngredient = new Ingredient(Guid.NewGuid(), "Tuz", command.Request.CategoryId, MeasurementCategory.Weight);
        var mappedResponse = new CreatedIngredientResponse { Id = mappedIngredient.Id, Name = mappedIngredient.Name };

        _mockIngredientRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        _mockMapper
            .Setup(m => m.Map<Ingredient>(command.Request))
            .Returns(mappedIngredient);

        _mockIngredientRepository
            .Setup(r => r.AddAsync(mappedIngredient, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mappedIngredient);

        _mockMapper
            .Setup(m => m.Map<CreatedIngredientResponse>(mappedIngredient))
            .Returns(mappedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(mappedIngredient.Id);
        result.Name.Should().Be("Tuz");

        _mockIngredientRepository.Verify(r => r.AddAsync(mappedIngredient, It.IsAny<CancellationToken>()), Times.Once);
    }
}
