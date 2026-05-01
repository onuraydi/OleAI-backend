using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeImage : Entity<Guid>
{
    public Guid RecipeId { get; set; }
    public string ImageUrl { get; set; }
    public string PublicId { get; set; } // Cloudinary vb. servislerden silmek için
    public bool IsMain { get; set; } // Kapak fotoğrafı mı?
    public Guid? StepId { get; set; } // Opsiyonel: Bir adıma aitse ID'si

    public RecipeImage() { }

    public RecipeImage(Guid id, Guid recipeId, string imageUrl, string publicId, bool isMain, Guid? stepId) : this()
    {
        Id = id;
        RecipeId = recipeId;
        ImageUrl = imageUrl;
        PublicId = publicId;
        IsMain = isMain;
        StepId = stepId;
    }
}
