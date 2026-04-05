using MediatR;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Pantry.EventHandlers
{
    public class InventoryDeductionEventHandler : INotificationHandler<RecipeConsumedEvent>
    {
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly IMediator _mediator; // yeni event fırlatmak için

        public InventoryDeductionEventHandler(IUserIngredientRepository userIngredientRepository, IMediator mediator)
        {
            _userIngredientRepository = userIngredientRepository;
            _mediator = mediator;
        }

        public async Task Handle(RecipeConsumedEvent notification, CancellationToken cancellationToken)
        {
            var consumedIngredientIds = notification.ConsumedIngredients.Keys.ToList();


            QueryOptions<UserIngredient> query = new QueryOptions<UserIngredient>();
            query.Predicate = ui => ui.UserId == notification.UserId && ui.IngredientId != null && consumedIngredientIds.Contains(ui.IngredientId.Value);
            query.EnableTracking = true;

            var pantryItemsResult = await _userIngredientRepository.GetListAsync(query, cancellationToken);

            var pantryItems = pantryItemsResult.Items;

            foreach (var item in pantryItems)
            {
                var amountNeeded = notification.ConsumedIngredients[item.IngredientId!.Value];

                // Bİten malzeme - Alışveriş listesine ekleme eklenecek
                if (item.Quantity <= amountNeeded)
                {
                    await _userIngredientRepository.DeleteAsync(item);

                    await _mediator.Publish(new IngredientDepletedEvent
                    {
                        UserId = notification.UserId,
                        MasterIngredientId = item.IngredientId.Value,
                        IngredientName = item.Ingredient?.Name ?? "Non"
                    }, cancellationToken);
                }
                else
                {
                    item.Consume(amountNeeded);
                    await _userIngredientRepository.UpdateAsync(item);
                }
            }
        }
    }
}
