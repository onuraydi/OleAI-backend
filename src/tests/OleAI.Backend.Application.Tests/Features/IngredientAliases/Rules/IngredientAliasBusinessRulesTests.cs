using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Rules
{
    public class IngredientAliasBusinessRulesTests
    {
        private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly IngredientAliasBusinessRules _businessRules;

        public IngredientAliasBusinessRulesTests()
        {
            _mockAliasRepository = new Mock<IIngredientAliasRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _businessRules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);
        }

        [Fact]
        public void AliasMustExistWhenRequested_WhenAliasIsNull_ShouldThrowNotFoundException()
        {
            // Act
            Action act = () => _businessRules.AliasMustExistWhenRequested(null);

            // Assert
            act.Should().Throw<NotFoundException>()
                .WithMessage(IngredientAliasMessages.AliasNotFoundMessage);
        }

        [Fact]
        public void AliasMustExistWhenRequested_WhenAliasIsNotNull_ShouldNotThrowException()
        {
            // Arrange
            var alias = new IngredientAlias { Id = Guid.NewGuid(), AliasName = "Domat" };

            // Act
            Action act = () => _businessRules.AliasMustExistWhenRequested(alias);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public async Task MasterIngredientMustExist_WhenIngredientNotFound_ShouldThrowBusinessException()
        {
            // Arrange
            var ingredientId = Guid.NewGuid();
            _mockIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Act
            Func<Task> act = async () => await _businessRules.MasterIngredientMustExist(ingredientId);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage(IngredientAliasMessages.MasterMaterialNotFoundMessage);
        }

        [Fact]
        public async Task AliasNameCannotBeDuplicated_WhenAliasExists_ShouldThrowBusinessException()
        {
            // Arrange
            var aliasName = "Domat";
            _mockAliasRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IngredientAlias { AliasName = "Domat" });

            // Act
            Func<Task> act = async () => await _businessRules.AliasNameCannotBeDuplicated(aliasName);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage($"'{aliasName}' kelimesi zaten sistemde eşanlamlı olarak kayıtlı.");
        }

        [Fact]
        public async Task AliasNameCannotBeDuplicated_WhenAliasMatchesMasterIngredient_ShouldThrowBusinessException()
        {
            // Arrange
            var aliasName = "Domates";
            
            _mockAliasRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IngredientAlias)null!);

            _mockIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Ingredient { Name = "Domates" });

            // Act
            Func<Task> act = async () => await _businessRules.AliasNameCannotBeDuplicated(aliasName);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage($"'{aliasName}' kelimesi sistemde zaten bir Ana Malzeme olarak bulunuyor, eşanlamlı olarak eklenemez.");
        }

        [Fact]
        public async Task AliasNameCannotBeDuplicated_WhenNameIsUnique_ShouldNotThrowException()
        {
            // Arrange
            var aliasName = "Eşsiz İsim";
            
            _mockAliasRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientAlias, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IngredientAlias)null!);

            _mockIngredientRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Ingredient, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient)null!);

            // Act
            Func<Task> act = async () => await _businessRules.AliasNameCannotBeDuplicated(aliasName);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
