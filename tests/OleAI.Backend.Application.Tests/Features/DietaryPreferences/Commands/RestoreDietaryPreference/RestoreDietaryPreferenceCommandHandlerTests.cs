using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.RestoreDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.DietaryPreferences.Commands.RestoreDietaryPreference;

public class RestoreDietaryPreferenceCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRestoreDietaryPreference()
    {
        var mockRepo = new Mock<IDietaryPreferenceRepository>();
        var rules = new DietaryPreferenceBusinessRules(mockRepo.Object);
        var handler = new RestoreDietaryPreferenceCommandHandler(mockRepo.Object, rules);

        var id = Guid.NewGuid();
        var command = new RestoreDietaryPreferenceCommand { Id = id };
        var preference = new DietaryPreference { Id = id, Name = "N", Description = "D", DeletedDate = DateTime.UtcNow };

        mockRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<DietaryPreference, bool>>>(), null, true, true, It.IsAny<CancellationToken>())).ReturnsAsync(preference);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        preference.DeletedDate.Should().BeNull();
        mockRepo.Verify(r => r.UpdateAsync(preference, It.IsAny<CancellationToken>()), Times.Once);
    }
}
