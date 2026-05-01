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
    public IReadOnlyCollection<RecipeIngredient> RecipeIngredients => _recipeIngredients;

    private readonly List<RecipeStep> _recipeSteps = new();
    public IReadOnlyCollection<RecipeStep> RecipeSteps => _recipeSteps;

    private readonly List<RecipeImage> _recipeImages = new();
    public IReadOnlyCollection<RecipeImage> RecipeImages => _recipeImages;


    public virtual ICollection<RecipeComment> Comments { get; private set; }
    public virtual ICollection<RecipeInteraction> Interactions { get; private set; }

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

    public void UpdateDetails(string title, string description, RecipeVisibility visibility)
    {
        Title = title;
        Description = description;
        SetVisibilityAndStatus(visibility);
    }

    /// <summary>
    /// Malzemeleri akıllıca senkronize eder (Eskileri siler, mevcutları günceller, yenileri ekler).
    /// </summary>
    public void SyncIngredients(IEnumerable<(Guid IngredientId, decimal Amount, int UnitId)> incomingIngredients)
    {
        var incomingIds = incomingIngredients.Select(x => x.IngredientId).ToList();

        _recipeIngredients.RemoveAll(i => !incomingIds.Contains(i.IngredientId));

        foreach (var incoming in incomingIngredients)
        {
            var existing = _recipeIngredients.FirstOrDefault(i => i.IngredientId == incoming.IngredientId);
            if (existing != null)
            {
                existing.Amount = incoming.Amount;
                existing.UnitId = incoming.UnitId;
            }
            else
            {
                _recipeIngredients.Add(new RecipeIngredient(Guid.NewGuid(), Id, incoming.IngredientId, incoming.Amount, incoming.UnitId));
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
                _recipeSteps.Add(new RecipeStep(Guid.NewGuid(), Id, incoming.StepNumber, incoming.Instruction));
        }

        return imagesToDeleteFromCloud;
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

        _recipeImages.Add(new RecipeImage(Guid.NewGuid(), Id, newImageUrl, newPublicId, isMain, stepId));

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
        if (Status != RecipeStatus.PendingAdminApproval)
            throw new Exception(DomainConstants.OnlyLlmApprovedRecipesCanBeApprovedByAdmin);

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
