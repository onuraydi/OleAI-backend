using MediatR;
using OleAI.Backend.Application.Features.Pantry.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Pantry.Commands.ResolveRequest
{
    public class ResolveIngredientRequestCommandHandler : IRequestHandler<ResolveIngredientRequestCommand, string>
    {
        private readonly IIngredientRequestRepository _ingredientRequestRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IIngredientAliasRepository _ingredientAliasRepository;
        private readonly IUserIngredientRepository _userIngredientRepository;

        public ResolveIngredientRequestCommandHandler(IIngredientRequestRepository ingredientRequestRepository, IIngredientRepository ingredientRepository, IIngredientAliasRepository ingredientAliasRepository, IUserIngredientRepository userIngredientRepository)
        {
            _ingredientRequestRepository = ingredientRequestRepository;
            _ingredientRepository = ingredientRepository;
            _ingredientAliasRepository = ingredientAliasRepository;
            _userIngredientRepository = userIngredientRepository;
        }

        public async Task<string> Handle(ResolveIngredientRequestCommand command, CancellationToken cancellationToken)
        {
            var ingredientRequest = await _ingredientRequestRepository.GetAsync(r => r.Id == command.Request.RequestId);
            if (ingredientRequest == null)
                throw new NotFoundException(PantryMessages.RequestNotFoundMessage);

            Guid targetMasterIngredientId = Guid.Empty;

            // Aksiyon 1: Yepyeni Bir Malzeme Olarak Ekle
            if (command.Request.Action == ResolutionAction.ApproveAsNew)
            {
                if (!command.Request.NewCategoryId.HasValue || !command.Request.NewUnitFamily.HasValue)
                    throw new BusinessException(PantryMessages.CategoryAndUnitTypeRequiredMessage);

                var newIngredient = new Ingredient(Guid.NewGuid(), ingredientRequest.RequestedName, command.Request.NewCategoryId.Value, command.Request.NewUnitFamily.Value);
                await _ingredientRepository.AddAsync(newIngredient);

                targetMasterIngredientId = newIngredient.Id;
                ingredientRequest.ResolveAsApproved();
            }
            // Aksiyon 2: Mevcut Bir Malzemenin Eşanlamlısı(Alias) Olarak Birleştir
            else if (command.Request.Action == ResolutionAction.MergeAsAlias)
            {
                if (!command.Request.MasterIngredientId.HasValue)
                    throw new BusinessException(PantryMessages.MasterMaterialRequiredForMergeMessage);

                var newAlias = new IngredientAlias(Guid.NewGuid(), command.Request.MasterIngredientId.Value, ingredientRequest.RequestedName);
                await _ingredientAliasRepository.AddAsync(newAlias);

                targetMasterIngredientId = command.Request.MasterIngredientId.Value;
                ingredientRequest.ResolveAsMerged(command.Request.AdminNote ?? PantryMessages.MergedAsAliasMessage);
            }
            // Aksiyon 3: Reddet (Saçma metin)
            else if (command.Request.Action == ResolutionAction.Reject)
            {
                ingredientRequest.Reject(command.Request.AdminNote ?? PantryMessages.RequestRejectedMessage);
                await _ingredientRequestRepository.UpdateAsync(ingredientRequest);
                return PantryMessages.RequestRejectedMessage;
            }

            await _ingredientRequestRepository.UpdateAsync(ingredientRequest);

            // Eğer onaylandıysa veya birleştirildiyse, kullanıcının dolabında "Onay Bekliyor" statüsünde olan bu ürünü AKTİF hale getiriyoruz.
            var userInventoryItem = await _userIngredientRepository.GetAsync(ui => ui.IngredientRequestId == command.Request.RequestId);
            if (userInventoryItem != null)
            {
                var existingMasterItem = await _userIngredientRepository.GetAsync(ui => ui.UserId == userInventoryItem.UserId && ui.IngredientId == targetMasterIngredientId);

                if (existingMasterItem != null)
                {
                    // Kullanıcının envanterinde bu ürün zaten var. Miktarları birleştir (aynı birimdeyse) ve pending olanı sil.
                    if (existingMasterItem.Unit == userInventoryItem.Unit && userInventoryItem.Amount > 0)
                    {
                        existingMasterItem.AddQuantity(userInventoryItem.Amount);
                        await _userIngredientRepository.UpdateAsync(existingMasterItem);
                    }

                    await _userIngredientRepository.DeleteAsync(userInventoryItem, permanent: true); // Pending kaydını kalıcı olarak sil.
                }
                else
                {
                    userInventoryItem.ApprovePendingRequest(targetMasterIngredientId);
                    await _userIngredientRepository.UpdateAsync(userInventoryItem);
                }
            }

            return PantryMessages.RequestResolvedAndInventoryUpdatedMessage;
        }
    }
}
