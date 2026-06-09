using MediatR;

namespace OleAI.Backend.Application.Features.Ingredients.Queries.GetPopularDislikedIngredients;

public class GetPopularDislikedIngredientsQuery : IRequest<List<GetPopularDislikedIngredientsResponse>>
{
    // Opsiyonel olarak bir kategori filtresi eklenebilir, şu an gerek yok tümünü çekeceğiz.
}
