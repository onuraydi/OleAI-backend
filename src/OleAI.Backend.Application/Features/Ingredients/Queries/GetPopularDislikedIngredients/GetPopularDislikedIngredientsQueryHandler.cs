using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Ingredients.Queries.GetPopularDislikedIngredients;

public class GetPopularDislikedIngredientsQueryHandler : IRequestHandler<GetPopularDislikedIngredientsQuery, List<GetPopularDislikedIngredientsResponse>>
{
    private readonly IIngredientRepository _ingredientRepository;

    public GetPopularDislikedIngredientsQueryHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<List<GetPopularDislikedIngredientsResponse>> Handle(GetPopularDislikedIngredientsQuery request, CancellationToken cancellationToken)
    {
        // Kutuplaştırıcı olarak işaretlenmişleri ve en çok sevilmeyenleri çekeceğiz.
        // Şimdilik sadece Admin'in "IsPolarizing" olarak işaretlediği malzemeleri döndürüyoruz.
        // İleride topluluk verisi birikince, UserDislikedIngredients tablosundan 
        // GroupBy ile en çok sevilmeyen top 20'yi çekip bununla birleştirebiliriz.

        var ingredients = await _ingredientRepository.GetListAsync(
            new OleAI.Backend.Core.Repositories.Options.QueryOptions<OleAI.Backend.Domain.Entities.Ingredient>
            {
                Predicate = i => i.IsPolarizing,
                Include = i => i.Include(x => x.Category),
                OrderBy = i => i.OrderBy(x => x.Name),
                Size = 100 // Get all polarizing up to 100
            },
            cancellationToken: cancellationToken
        );

        var response = ingredients.Items.Select(i => new GetPopularDislikedIngredientsResponse
        {
            Id = i.Id,
            Name = i.Name,
            CategoryName = i.Category.Name
        }).ToList();

        return response;
    }
}
