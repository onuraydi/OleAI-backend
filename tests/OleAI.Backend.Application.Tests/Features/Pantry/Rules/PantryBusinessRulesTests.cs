using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Rules
{
    public class PantryBusinessRulesTests
    {
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
        private readonly PantryBusinessRules _businessRules;

        public PantryBusinessRulesTests()
        {
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
            _businessRules = new PantryBusinessRules(_mockIngredientRepository.Object, _mockUserIngredientRepository.Object, _mockIngredientRequestRepository.Object);
        }

        [Fact]
        public void CannotSendBothIdAndName_Should_ThrowBusinessException_When_BothAreProvided()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            var requestedName = "Yeni Ürün";

            // Act & Assert
            Action action = () => _businessRules.CannotSendBothIdAndName(ingredientId, requestedName);
            action.Should().Throw<BusinessException>().WithMessage("Aynı anda hem listeden ürün seçip hem de özel isim giremezsiniz.");
        }

        [Fact]
        public void CannotSendBothIdAndName_Should_ThrowBusinessException_When_NeitherIsProvided()
        {
            // Arrange & Act & Assert
            Action action = () => _businessRules.CannotSendBothIdAndName(null, "");
            action.Should().Throw<BusinessException>().WithMessage("Lütfen bir ürün seçin veya adını yazın.");
        }

        [Fact]
        public async Task IngredientMustExistWhenSelectedFromList_Should_ThrowBusinessException_When_NotFound()
        {
            // Arrange
            _mockIngredientRepository.Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IIncludableQueryable<Ingredient, object>>?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient?)null);

            // Act & Assert
            Func<Task> action = async () => await _businessRules.IngredientMustExistWhenSelectedFromList(Guid.NewGuid());
            await action.Should().ThrowAsync<BusinessException>().WithMessage("Seçilen malzeme sistemde bulunamadı.");
        }
    }
}
