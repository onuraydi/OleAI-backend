namespace OleAI.Backend.Application.Features.Pantry.Dtos.Requests
{
    public class AddMultipleUserIngredientsRequest
    {
        public List<AddUserIngredientRequest> Ingredients { get; set; } = new();
    }
}
