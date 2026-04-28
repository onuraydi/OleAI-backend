using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeStep : Entity<Guid>
{
    public Guid RecipeId { get; set; }
    public int StepNumber { get; set; }
    public string Instruction { get; set; }

    public RecipeStep() { }

    public RecipeStep(Guid id, Guid recipeId, int stepNumber, string instruction) : this()
    {
        Id = id;
        RecipeId = recipeId;
        StepNumber = stepNumber;
        Instruction = instruction;
    }
}
