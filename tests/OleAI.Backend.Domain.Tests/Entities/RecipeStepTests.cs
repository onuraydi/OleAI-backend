using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class RecipeStepTests
{
    [Fact]
    public void ApplyLlmCorrection_ShouldUpdateInstruction()
    {
        var step = new RecipeStep(Guid.NewGuid(), Guid.NewGuid(), 1, "Old Instruction");

        step.ApplyLlmCorrection("New Instruction");

        step.Instruction.Should().Be("New Instruction");
    }
}
