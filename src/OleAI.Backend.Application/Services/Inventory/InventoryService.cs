using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Services.Inventory
{
    public class InventoryService
    {
        /// <summary>
        /// Tarifte kullanılan malzemeleri envanterden düşer ve eksilenleri belirler.
        /// </summary>
        public List<Guid> DeductIngredientsFromPantry(IEnumerable<UserIngredient> userIngredients, IEnumerable<RecipeIngredient> recipeIngredients)
        {
            var itemsToAddToShoppingList = new List<Guid>();

            foreach (var recipeIngredient in recipeIngredients)
            {
                var userIngredient = userIngredients.FirstOrDefault(ui => ui.IngredientId == recipeIngredient.IngredientId);

                if (userIngredient != null)
                {
                    // TODO: BURada tür dönüşümü yüzünden ufak yuvarlama hataları alınabilir.
                    userIngredient.DecreaseAmount((double)recipeIngredient.Amount);

                    if (userIngredient.IsDepleted)
                    {
                        if (recipeIngredient.IngredientId.HasValue)
                        {
                            itemsToAddToShoppingList.Add(recipeIngredient.IngredientId.Value);
                        }
                    }
                }
                else
                {
                    if (recipeIngredient.IngredientId.HasValue)
                    {
                        itemsToAddToShoppingList.Add(recipeIngredient.IngredientId.Value);
                    }
                }
            }
            return itemsToAddToShoppingList;
        }
    }
}
