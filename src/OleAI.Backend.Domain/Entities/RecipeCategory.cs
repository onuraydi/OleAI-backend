using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities;

public class RecipeCategory : Entity<int>
{
    public string Name { get; private set; }
    public string IconName { get; private set; }

    public virtual ICollection<RecipeCategoryMapping> RecipeCategories { get; private set; }

    private RecipeCategory()
    {
        Name = string.Empty;
        IconName = "category";
        RecipeCategories = new HashSet<RecipeCategoryMapping>();
    }

    public RecipeCategory(int id, string name, string iconName = "category") : this()
    {
        Id = id;
        Name = name;
        IconName = iconName;
    }
}
