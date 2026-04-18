using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.ShoppingList.Commands.AddShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Commands.MarkAsPurchased;
using OleAI.Backend.Application.Features.ShoppingList.Commands.RemoveShoppingListItem;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Application.Features.ShoppingList.Queries.GetMyShoppingList;

namespace OleAI.Backend.Presentation.Controllers;

[Route("api/shopping-list")]
[ApiController]
[Authorize]
public class ShoppingListController : BaseController
{
    private Guid LoggedInUserId => Guid.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı."));

    /// <summary>
    /// Kullanıcının aktif (satın alınmamış) alışveriş listesini getirir.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyShoppingList()
    {
        var query = new GetMyShoppingListQuery { UserId = LoggedInUserId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Alışveriş listesine manuel olarak yeni bir malzeme ekler.
    /// Aynı malzeme zaten listede (satın alınmamış) varsa 409 döner.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] AddShoppingListItemRequest request)
    {
        // Güvenlik: Client'tan gelen UserId'yi eziyoruz, token'dan aldığımız gerçek ID'yi kullanıyoruz.
        request.UserId = LoggedInUserId;

        var command = new AddShoppingListItemCommand { Request = request };
        var result = await Mediator.Send(command);

        return Created($"api/shopping-list/{result.Id}", result);
    }

    /// <summary>
    /// Alışveriş listesindeki bir ürünü satın alındı olarak işaretler.
    /// </summary>
    [HttpPatch("{id:guid}/purchase")]
    public async Task<IActionResult> MarkAsPurchased([FromRoute] Guid id)
    {
        var command = new MarkAsPurchasedCommand
        {
            ItemId = id,
            UserId = LoggedInUserId
        };

        var result = await Mediator.Send(command);
        return Ok(new { Message = result });
    }

    /// <summary>
    /// Alışveriş listesinden bir ürünü siler.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid id)
    {
        var command = new RemoveShoppingListItemCommand
        {
            ItemId = id,
            UserId = LoggedInUserId
        };

        var result = await Mediator.Send(command);
        return Ok(new { Message = result });
    }
}
