using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands
{
    public class AddUserIngredientCommandHandlerTests
    {
        private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
        private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly PantryBusinessRules _businessRules;
        private readonly AddUserIngredientCommandHandler _handler;

        public AddUserIngredientCommandHandlerTests()
        {
            _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
            _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();

            _businessRules = new PantryBusinessRules(_mockIngredientRepository.Object);
            _handler = new AddUserIngredientCommandHandler(
                _mockUserIngredientRepository.Object,
                _mockIngredientRequestRepository.Object,
                _businessRules);
        }

        [Fact]
        public async Task Handle_Should_IncreaseQuantity_When_IngredientAlreadyInPantry()
        {
            // Arrange
            var addUserIngredientRequest = new AddUserIngredientRequest
            {
                UserId = Guid.NewGuid(),
                IngredientId = Guid.NewGuid(),
                Quantity = 500
            };

            var command = new AddUserIngredientCommand
            {
                Request = addUserIngredientRequest,
            };

            // DÜZELTME 1: GetAsync için 5 parametre (araya enableTracking için 'true' veya 'false' eklendi)
            _mockIngredientRepository.Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    null,
                    false,
                    true,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Ingredient(command.Request.IngredientId.Value, "Domates", Guid.NewGuid(), "Gr"));

            // Dolapta aynı ürün varmış gibi davran (İçinde 200 gram var)
            var existingPantryItem = UserIngredient.CreateStandard(Guid.NewGuid(), command.Request.UserId, command.Request.IngredientId.Value, 200);

            // DÜZELTME 2: GetAsync için 5 parametre
            _mockUserIngredientRepository.Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<UserIngredient, bool>>>(),
                    null,
                    false,
                    true,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPantryItem);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert (Doğrulama)
            result.Should().NotBeNull();
            result.IsPendingApproval.Should().BeFalse();
            result.Message.Should().Be("Ürün miktarı güncellendi.");

            // Dolaptaki ürünün miktarı 200 + 500 = 700 olmuş mu?
            existingPantryItem.Quantity.Should().Be(700);

            // DÜZELTME 3: UpdateAsync ve AddAsync için It.IsAny<CancellationToken>() eklendi
            _mockUserIngredientRepository.Verify(r => r.UpdateAsync(existingPantryItem, It.IsAny<CancellationToken>()), Times.Once);
            _mockUserIngredientRepository.Verify(r => r.AddAsync(It.IsAny<UserIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_CreateIngredientRequest_When_CustomNameProvided()
        {
            // Arrange: Kullanıcı listede olmayan serbest bir metin girdi
            var addUserIngredientRequest = new AddUserIngredientRequest
            {
                UserId = Guid.NewGuid(),
                RequestedName = "Ejder Meyvesi",
                Quantity = 2
            };

            var command = new AddUserIngredientCommand { Request = addUserIngredientRequest };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsPendingApproval.Should().BeTrue();
            result.Message.Should().Contain("yapay zeka onayına gönderildi");

            // DÜZELTME 4: AddAsync metoduna It.IsAny<CancellationToken>() eklendi
            _mockIngredientRequestRepository.Verify(r => r.AddAsync(
                It.Is<IngredientRequest>(ir => ir.RequestedName == "Ejder Meyvesi"),
                It.IsAny<CancellationToken>()), Times.Once);

            // DÜZELTME 5: AddAsync metoduna It.IsAny<CancellationToken>() eklendi
            _mockUserIngredientRepository.Verify(r => r.AddAsync(
                It.Is<UserIngredient>(ui => ui.IngredientId == null && ui.Quantity == 2),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
