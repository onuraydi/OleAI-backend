using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Commands.CreateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.Ingredients.Commands
{
    public class CreateIngredientCommandHandlerTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientBusinessRules _businessRules;
        private readonly CreateIngredientCommandHandler _handler;

        public CreateIngredientCommandHandlerTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _businessRules = new IngredientBusinessRules(_mockIngredientRepository.Object);

            _handler = new CreateIngredientCommandHandler(
                _mockIngredientRepository.Object, 
                _mockMapper.Object, 
                _businessRules);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldCreateIngredientAndReturnResponse()
        {
            // Arrange
            var request = new CreateIngredientRequest 
            { 
                Name = "Tomato", 
                CategoryId = Guid.NewGuid(), 
                BaseUnitType = "kg" 
            };
            var command = new CreateIngredientCommand { Request = request };

            var mappedIngredient = new Ingredient { Name = "Tomato", CategoryId = request.CategoryId };
            var createdIngredient = new Ingredient { Id = Guid.NewGuid(), Name = "Tomato", CategoryId = request.CategoryId };
            var expectedResponse = new CreatedIngredientResponse { Id = createdIngredient.Id, Name = "Tomato" };

            // Mock business rules duplicate check
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            _mockMapper.Setup(m => m.Map<Ingredient>(request)).Returns(mappedIngredient);
            _mockIngredientRepository.Setup(r => r.AddAsync(mappedIngredient)).ReturnsAsync(createdIngredient);
            _mockMapper.Setup(m => m.Map<CreatedIngredientResponse>(createdIngredient)).Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(createdIngredient.Id);
            result.Name.Should().Be("Tomato");

            _mockIngredientRepository.Verify(r => r.AddAsync(It.IsAny<Ingredient>()), Times.Once);
            _mockMapper.Verify(m => m.Map<Ingredient>(request), Times.Once);
            _mockMapper.Verify(m => m.Map<CreatedIngredientResponse>(createdIngredient), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenIngredientNameAlreadyExists_ShouldThrowBusinessException()
        {
            // Arrange
            var request = new CreateIngredientRequest { Name = "Tomato" };
            var command = new CreateIngredientCommand { Request = request };
            var existingIngredient = new Ingredient { Id = Guid.NewGuid(), Name = "Tomato", DeletedDate = null };

            // Mock business rules duplicate check
            _mockIngredientRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingIngredient);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(IngredientMessages.ActiveIngredientAlreadyExistsMessage);

            _mockIngredientRepository.Verify(r => r.AddAsync(It.IsAny<Ingredient>()), Times.Never);
        }
    }
}
