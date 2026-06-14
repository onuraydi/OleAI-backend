using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities;

public class Recipe : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public virtual User? User { get; set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public RecipeDifficulty Difficulty { get; private set; }
    public int Servings { get; private set; }
    public int PrepTime { get; private set; }
    public int CookTime { get; private set; }
    public RecipeVisibility Visibility { get; private set; }
    public RecipeStatus Status { get; private set; }

    // Dışarıdan .Add() ile rastgele veri eklenmesini engelliyoruz
    private readonly List<RecipeIngredient> _recipeIngredients = new();
    public IReadOnlyCollection<RecipeIngredient> RecipeIngredients => _recipeIngredients;

    private readonly List<RecipeStep> _recipeSteps = new();
    public IReadOnlyCollection<RecipeStep> RecipeSteps => _recipeSteps;

    private readonly List<RecipeImage> _recipeImages = new();
    public IReadOnlyCollection<RecipeImage> RecipeImages => _recipeImages;

    private readonly List<RecipeCategoryMapping> _recipeCategories = new();
    public IReadOnlyCollection<RecipeCategoryMapping> RecipeCategories => _recipeCategories;

    private readonly List<RecipeTag> _recipeTags = new();
    public IReadOnlyCollection<RecipeTag> RecipeTags => _recipeTags;


    public virtual ICollection<RecipeComment> Comments { get; private set; }
    public virtual ICollection<RecipeInteraction> Interactions { get; private set; }

    private Recipe() { } // EF Core için

    // Factory Method: Yeni tarif sadece bu yolla yaratılır
    public static Recipe Create(Guid userId, string title, string description, RecipeDifficulty difficulty, int servings, int prepTime, int cookTime, RecipeVisibility visibility)
    {
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Description = description,
            Difficulty = difficulty,
            Servings = servings,
            PrepTime = prepTime,
            CookTime = cookTime
        };
        recipe.SetVisibilityAndStatus(visibility);
        return recipe;
    }

    public void UpdateDetails(string title, string description, RecipeDifficulty difficulty, int servings, int prepTime, int cookTime, RecipeVisibility visibility)
    {
        Title = title;
        Description = description;
        Difficulty = difficulty;
        Servings = servings;
        PrepTime = prepTime;
        CookTime = cookTime;
        SetVisibilityAndStatus(visibility);
    }

    /// <summary>
    /// Malzemeleri akıllıca senkronize eder (Eskileri siler, mevcutları günceller, yenileri ekler).
    /// </summary>
    public void SyncIngredients(IEnumerable<(Guid? IngredientId, Guid? IngredientRequestId, decimal Amount, int UnitId)> incomingIngredients)
    {
        var incomingIngredientIds = incomingIngredients.Where(x => x.IngredientId.HasValue).Select(x => x.IngredientId).ToList();
        var incomingRequestIds = incomingIngredients.Where(x => x.IngredientRequestId.HasValue).Select(x => x.IngredientRequestId).ToList();

        _recipeIngredients.RemoveAll(i =>
            (i.IngredientId.HasValue && !incomingIngredientIds.Contains(i.IngredientId)) ||
            (i.IngredientRequestId.HasValue && !incomingRequestIds.Contains(i.IngredientRequestId))
        );

        foreach (var incoming in incomingIngredients)
        {
            RecipeIngredient? existing = null;
            if (incoming.IngredientId.HasValue)
                existing = _recipeIngredients.FirstOrDefault(i => i.IngredientId == incoming.IngredientId);
            else if (incoming.IngredientRequestId.HasValue)
                existing = _recipeIngredients.FirstOrDefault(i => i.IngredientRequestId == incoming.IngredientRequestId);

            if (existing != null)
            {
                existing.Amount = incoming.Amount;
                existing.UnitId = incoming.UnitId;
            }
            else
            {
                _recipeIngredients.Add(new RecipeIngredient(Guid.Empty, Id, incoming.IngredientId, incoming.IngredientRequestId, incoming.Amount, incoming.UnitId));
            }
        }
    }

    /// <summary>
    /// Adımları senkronize eder. Eğer bir adım silinmişse ve o adıma ait bir resim varsa,
    /// o resmin Cloudinary'den silinmesi için PublicId'sini döner.
    /// </summary>
    public List<string> SyncSteps(IEnumerable<(int StepNumber, string Instruction)> incomingSteps)
    {
        var incomingNumbers = incomingSteps.Select(x => x.StepNumber).ToList();
        var imagesToDeleteFromCloud = new List<string>();

        var removedSteps = _recipeSteps.Where(s => !incomingNumbers.Contains(s.StepNumber)).ToList();
        var removedStepIds = removedSteps.Select(s => s.Id).ToList();

        var orphanedImages = _recipeImages.Where(i => i.StepId.HasValue && removedStepIds.Contains(i.StepId.Value)).ToList();
        imagesToDeleteFromCloud.AddRange(orphanedImages.Select(i => i.PublicId));

        _recipeSteps.RemoveAll(s => removedStepIds.Contains(s.Id));
        _recipeImages.RemoveAll(i => orphanedImages.Contains(i));

        foreach (var incoming in incomingSteps)
        {
            var existing = _recipeSteps.FirstOrDefault(s => s.StepNumber == incoming.StepNumber);
            if (existing != null)
                existing.Instruction = incoming.Instruction;
            else
                _recipeSteps.Add(new RecipeStep(Guid.Empty, Id, incoming.StepNumber, incoming.Instruction));
        }

        return imagesToDeleteFromCloud;
    }

    public void SyncCategories(IEnumerable<int> categoryIds)
    {
        var incoming = categoryIds.ToList();
        _recipeCategories.RemoveAll(c => !incoming.Contains(c.RecipeCategoryId));

        foreach (var id in incoming)
        {
            if (!_recipeCategories.Any(c => c.RecipeCategoryId == id))
            {
                _recipeCategories.Add(new RecipeCategoryMapping(Guid.Empty, Id, id));
            }
        }
    }

    public void SyncTags(IEnumerable<int> tagIds)
    {
        var incoming = tagIds.ToList();
        _recipeTags.RemoveAll(t => !incoming.Contains(t.TagId));

        foreach (var id in incoming)
        {
            if (!_recipeTags.Any(t => t.TagId == id))
            {
                _recipeTags.Add(new RecipeTag(Guid.Empty, Id, id));
            }
        }
    }

    /// <summary>
    /// Ana resmi veya adım resmini değiştirir, buluttan silinmesi için eski PublicId'yi döner.
    /// </summary>
    public string? ReplaceImage(string newImageUrl, string newPublicId, bool isMain, Guid? stepId = null)
    {
        string? oldPublicId = null;

        var existingImage = isMain
            ? _recipeImages.FirstOrDefault(i => i.IsMain)
            : _recipeImages.FirstOrDefault(i => i.StepId == stepId);

        if (existingImage != null)
        {
            oldPublicId = existingImage.PublicId;
            _recipeImages.Remove(existingImage);
        }

        _recipeImages.Add(new RecipeImage(Guid.Empty, Id, newImageUrl, newPublicId, isMain, stepId));

        return oldPublicId;
    }

    public void SetVisibilityAndStatus(RecipeVisibility visibility)
    {
        Visibility = visibility;

        Status = visibility == RecipeVisibility.Private
            ? RecipeStatus.Published
            : RecipeStatus.PendingLlmApproval;
    }

    public void ApproveByAdmin()
    {
        if (Status != RecipeStatus.PendingAdminApproval && Status != RecipeStatus.PendingLlmApproval)
            throw new Exception("Sadece onay bekleyen tarifler onaylanabilir.");

        Status = RecipeStatus.Published;
    }

    public void ForcePublish()
    {
        Status = RecipeStatus.Published;
    }

    public void Reject(string reason)
    {
        // Reddetme nedeni ileride bir log tablosuna yazılabilir.
        Status = RecipeStatus.Rejected;
    }

    public void PassLlmFilter()
    {
        if (Status == RecipeStatus.PendingLlmApproval)
            Status = RecipeStatus.PendingAdminApproval;
    }

    public void AddIngredient(Guid? ingredientId, Guid? ingredientRequestId, decimal amount, int unitId)
    {
        _recipeIngredients.Add(new RecipeIngredient(Guid.Empty, Id, ingredientId, ingredientRequestId, amount, unitId));
    }

    public void AddStep(int stepNumber, string instruction)
    {
        _recipeSteps.Add(new RecipeStep(Guid.Empty, Id, stepNumber, instruction));
    }

    public void AddImage(string imageUrl, string publicId, bool isMain, Guid? stepId = null)
    {
        _recipeImages.Add(new RecipeImage(Guid.Empty, Id, imageUrl, publicId, isMain, stepId));
    }
}
