using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.DeleteAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Constants;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Commands.DeleteAllergyRestriction;

public class DeleteAllergyRestrictionCommandHandlerTests
{
    private readonly Mock<IAllergyRestrictionRepository> _mockRepository;
    private readonly AllergyRestrictionBusinessRules _businessRules;
    private readonly DeleteAllergyRestrictionCommandHandler _handler;

    public DeleteAllergyRestrictionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAllergyRestrictionRepository>();
        _businessRules = new AllergyRestrictionBusinessRules(_mockRepository.Object);
        _handler = new DeleteAllergyRestrictionCommandHandler(_mockRepository.Object, _businessRules);
    }

    [Fact]
    public async Task Handle_WhenExists_ShouldDeleteAndReturnSuccessMessage()
    {
        // Arrange
        var command = new DeleteAllergyRestrictionCommand { Id = Guid.NewGuid() };
        var restriction = new AllergyRestriction { Id = command.Id, Name = "Gluten Allergy" };

        _mockRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<AllergyRestriction, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(restriction);

        _mockRepository
            .Setup(r => r.DeleteAsync(It.IsAny<AllergyRestriction>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AllergyRestriction ar, bool _, CancellationToken __) => ar);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(AllergyRestrictionMessages.AllergyRestrictionDeletedSuccessfully);
        _mockRepository.Verify(r => r.DeleteAsync(It.Is<AllergyRestriction>(ar => ar.Id == command.Id), false, It.IsAny<CancellationToken>()), Times.Once);
    }
}
