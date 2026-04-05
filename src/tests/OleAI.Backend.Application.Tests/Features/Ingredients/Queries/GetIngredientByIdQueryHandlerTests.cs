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
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Queries
{
    public class GetIngredientByIdQueryHandlerTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientBusinessRules _businessRules;
        private readonly GetIngredientByIdQueryHandler _handler;

        public GetIngredientByIdQueryHandlerTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _businessRules = new IngredientBusinessRules(_mockIngredientRepository.Object);

            _handler = new GetIngredientByIdQueryHandler(
                _mockIngredientRepository.Object, 
                _mockMapper.Object, 
                _businessRules);
        }

        [Fact]
        public async Task Handle_WhenIngredientExists_ShouldReturnIngredientResponse()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            var query = new GetIngredientByIdQuery { Id = ingredientId };
            var existingIngredient = new Ingredient { Id = ingredientId, Name = "Meat" };
            var expectedResponse = new IngredientResponse { Id = ingredientId, Name = "Meat" };

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingIngredient);

            _mockMapper.Setup(m => m.Map<IngredientResponse>(existingIngredient))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ingredientId);
            result.Name.Should().Be("Meat");

            _mockIngredientRepository.Verify(r => r.GetAsync(
                It.IsAny<Expression<Func<Ingredient, bool>>>(),
                null, false, true, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(m => m.Map<IngredientResponse>(existingIngredient), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenIngredientDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetIngredientByIdQuery { Id = Guid.NewGuid() };

            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(IngredientMessages.IngredientNotFoundMessage);
        }
    }
}
