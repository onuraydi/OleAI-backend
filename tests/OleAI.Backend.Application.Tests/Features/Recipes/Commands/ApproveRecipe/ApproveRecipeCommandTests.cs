using FluentAssertions;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommandTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var command = new ApproveRecipeCommand();
        var id = System.Guid.NewGuid();

        command.Id = id;
        command.EditedDescription = "Desc";
        command.EditedSteps = new System.Collections.Generic.List<EditedStepDto>();

        command.Id.Should().Be(id);
        command.EditedDescription.Should().Be("Desc");
        command.EditedSteps.Should().NotBeNull();

        var step = new EditedStepDto();
        step.StepNumber = 1;
        step.Instruction = "Inst";

        step.StepNumber.Should().Be(1);
        step.Instruction.Should().Be("Inst");
    }
}
