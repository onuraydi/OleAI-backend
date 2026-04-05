using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Commands.UpdateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Commands
{
    public class UpdateIngredientCommandHandlerTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientBusinessRules _businessRules;
        private readonly UpdateIngredientCommandHandler _handler;

        public UpdateIngredientCommandHandlerTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _businessRules = new IngredientBusinessRules(_mockIngredientRepository.Object);

            _handler = new UpdateIngredientCommandHandler(
                _mockIngredientRepository.Object, 
                _mockMapper.Object, 
                _businessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldUpdateIngredientAndReturnMessage()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            var request = new UpdateIngredientRequest { Id = ingredientId, Name = "Updated Tomato", CategoryId = Guid.NewGuid(), BaseUnitType = "kg" };
            var command = new UpdateIngredientCommand { Request = request };
            var existingIngredient = new Ingredient { Id = ingredientId, Name = "Tomato" };

            // Mock GetAsync to return the existing ingredient
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingIngredient);

            // Mock GetAsync for duplicate name check to return null
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.Is<Expression<Func<Ingredient, bool>>>(expr => expr.Compile().Invoke(new Ingredient { Name = "Updated Tomato" })),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            _mockIngredientRepository.Setup(r => r.UpdateAsync(existingIngredient)).ReturnsAsync(existingIngredient);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(IngredientMessages.IngredientUpdatedMessage);
            _mockMapper.Verify(m => m.Map(request, existingIngredient), Times.Once);
            _mockIngredientRepository.Verify(r => r.UpdateAsync(existingIngredient), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenIngredientNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new UpdateIngredientCommand { Request = new UpdateIngredientRequest { Id = Guid.NewGuid(), Name = "Tomato" } };

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(IngredientMessages.IngredientNotFoundMessage);

            _mockIngredientRepository.Verify(r => r.UpdateAsync(It.IsAny<Ingredient>()), Times.Never);
        }
    }
}
