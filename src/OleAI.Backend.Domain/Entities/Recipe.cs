using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities;

public class Recipe : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public RecipeVisibility Visibility { get; private set; }
    public RecipeStatus Status { get; private set; }

    // Dışarıdan .Add() ile rastgele veri eklenmesini engelliyoruz
    private readonly List<RecipeIngredient> _recipeIngredients = new();
    public IReadOnlyCollection<RecipeIngredient> RecipeIngredients => _recipeIngredients.AsReadOnly();

    private readonly List<RecipeStep> _recipeSteps = new();
    public IReadOnlyCollection<RecipeStep> RecipeSteps => _recipeSteps.AsReadOnly();

    private readonly List<RecipeImage> _recipeImages = new();
    public IReadOnlyCollection<RecipeImage> RecipeImages => _recipeImages.AsReadOnly();

    private Recipe() { } // EF Core için

    // Factory Method: Yeni tarif sadece bu yolla yaratılır
    public static Recipe Create(Guid userId, string title, string description, RecipeVisibility visibility)
    {
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Description = description
        };
        recipe.SetVisibilityAndStatus(visibility);
        return recipe;
    }

    // İş Kuralı: Visibility'e göre Status otomatik belirlenir
    public void SetVisibilityAndStatus(RecipeVisibility visibility)
    {
        Visibility = visibility;
        Status = visibility == RecipeVisibility.Private
            ? RecipeStatus.Published
            : RecipeStatus.PendingApproval;
    }

    public void AddIngredient(Guid ingredientId, decimal amount, int unitId)
    {
        _recipeIngredients.Add(new RecipeIngredient(Guid.NewGuid(), Id, ingredientId, amount, unitId));
    }

    public void AddStep(int stepNumber, string instruction)
    {
        _recipeSteps.Add(new RecipeStep(Guid.NewGuid(), Id, stepNumber, instruction));
    }

    public void AddImage(string imageUrl, string publicId, bool isMain, Guid? stepId = null)
    {
        _recipeImages.Add(new RecipeImage(Guid.NewGuid(), Id, imageUrl, publicId, isMain, stepId));
    }
}
