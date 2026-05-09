using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.DeleteDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Constants;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Commands.DeleteDietaryPreference;

public class DeleteDietaryPreferenceCommandHandlerTests
{
    private readonly Mock<IDietaryPreferenceRepository> _mockDietaryPreferenceRepository;
    private readonly DietaryPreferenceBusinessRules _businessRules;
    private readonly DeleteDietaryPreferenceCommandHandler _handler;

    public DeleteDietaryPreferenceCommandHandlerTests()
    {
        _mockDietaryPreferenceRepository = new Mock<IDietaryPreferenceRepository>();
        _businessRules = new DietaryPreferenceBusinessRules(_mockDietaryPreferenceRepository.Object);
        _handler = new DeleteDietaryPreferenceCommandHandler(_mockDietaryPreferenceRepository.Object, _businessRules);
    }

    [Fact]
    public async Task Handle_WhenPreferenceExists_ShouldDeleteAndReturnSuccessMessage()
    {
        // Arrange
        var command = new DeleteDietaryPreferenceCommand { Id = Guid.NewGuid() };
        var preference = new DietaryPreference { Id = command.Id, Name = "Vegan" };

        _mockDietaryPreferenceRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<DietaryPreference, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _mockDietaryPreferenceRepository
            .Setup(r => r.DeleteAsync(It.IsAny<DietaryPreference>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DietaryPreference dp, bool _, CancellationToken __) => dp);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(DietaryPreferenceMessages.DietaryPreferenceDeletedSuccessfully);
        _mockDietaryPreferenceRepository.Verify(r => r.DeleteAsync(It.Is<DietaryPreference>(dp => dp.Id == command.Id), false, It.IsAny<CancellationToken>()), Times.Once);
    }
}
