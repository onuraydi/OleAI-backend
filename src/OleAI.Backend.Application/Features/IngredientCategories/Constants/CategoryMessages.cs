namespace OleAI.Backend.Application.Features.IngredientCategories.Constants
{
    public static class CategoryMessages
    {
        public const string CategoryNotFoundMessage = "İstenilen kategori bulunamadı.";
        public const string CategoryAlreadyExistsMessage = "Bu isimde bir kategori zaten mevcut.";
        public const string CategoryCannotBeDeletedDueToDependenciesMessage = "Bu kategoriye bağlı malzemeler bulunduğu için silinemez. Önce malzemeleri başka kategoriye taşıyın.";
        public const string CreatedCategoryMessage = "Kategori başarıyla eklendi.";
        public const string UpdatedCategoryMessage = "Kategori başarıyla güncellendi.";
        public const string DeletedCategoryMessage = "Kategori başarıyla silindi.";
    }
}
