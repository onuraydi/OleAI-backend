using System;
using System.Collections.Generic;

namespace OleAI.Backend.Application.Features.Pantry.Dtos.Responses
{
    public class CategorizedPopularIngredientsResponse
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;

        public List<IngredientSearchResultResponse> Ingredients { get; set; } = new();
    }
}
