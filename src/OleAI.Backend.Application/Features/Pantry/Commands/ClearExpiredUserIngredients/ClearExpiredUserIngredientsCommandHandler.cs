using MediatR;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Pantry.Commands.ClearExpiredUserIngredients
{
    public class ClearExpiredUserIngredientsCommandHandler : IRequestHandler<ClearExpiredUserIngredientsCommand, string>
    {
        private readonly IUserIngredientRepository _userIngredientRepository;

        public ClearExpiredUserIngredientsCommandHandler(IUserIngredientRepository userIngredientRepository)
        {
            _userIngredientRepository = userIngredientRepository;
        }

        public async Task<string> Handle(ClearExpiredUserIngredientsCommand request, CancellationToken cancellationToken)
        {
            QueryOptions<UserIngredient> query = new QueryOptions<UserIngredient>();
            query.Predicate = ui => ui.UserId == request.userId && ui.ExpirationDate != null && ui.ExpirationDate < DateTime.UtcNow;
            query.EnableTracking = false;

            var expiredItemsResult = await _userIngredientRepository.GetListAsync(query, cancellationToken);
            var expiredItems = expiredItemsResult.Items;

            if (!expiredItems.Any())
                return PantryMessages.NoExpiredProductsToCleanMessage;

            //foreach (var item in expiredItems)
            //{
            //    // TODO Burada tarihi geçenleri belirtmek için eklenecek olan 'isExpired' alanı set edilebilir.
            //}

            await _userIngredientRepository.DeleteRangeAsync(expiredItems, permanent: false, cancellationToken: cancellationToken);

            return $"Tarihi geçmiş {expiredItems.Count} adet ürün dolabınızdan temizlendi";
        }
    }
}
