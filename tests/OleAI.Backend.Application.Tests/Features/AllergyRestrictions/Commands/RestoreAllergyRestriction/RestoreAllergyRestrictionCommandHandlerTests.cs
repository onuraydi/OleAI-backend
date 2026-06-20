using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.RestoreAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.AllergyRestrictions.Commands.RestoreAllergyRestriction;

public class RestoreAllergyRestrictionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRestoreAllergyRestriction()
    {
        var mockRepo = new Mock<IAllergyRestrictionRepository>();
        var rules = new AllergyRestrictionBusinessRules(mockRepo.Object);
        var handler = new RestoreAllergyRestrictionCommandHandler(mockRepo.Object, rules);

        var id = Guid.NewGuid();
        var command = new RestoreAllergyRestrictionCommand { Id = id };
        var restriction = new AllergyRestriction { Id = id, Name = "N", Description = "D", DeletedDate = DateTime.UtcNow };

        mockRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<AllergyRestriction, bool>>>(), null, true, true, It.IsAny<CancellationToken>())).ReturnsAsync(restriction);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        restriction.DeletedDate.Should().BeNull();
        mockRepo.Verify(r => r.UpdateAsync(restriction, It.IsAny<CancellationToken>()), Times.Once);
    }
}
