using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.RejectRecipe;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPendingAdminRecipes;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RecipesController : BaseController
{
    /// <summary>
    /// Sisteme görsel destekli yeni bir tarif ekler.
    /// </summary>
    /// <remarks>
    /// Bu endpoint form-data kabul eder. Resimler ve liste verileri aynı form içinde gönderilmelidir.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] CreateRecipeCommand createRecipeCommand)
    {
        string resultMessage = await Mediator.Send(createRecipeCommand);

        return Created(uri: "", new { Message = resultMessage });
    }

    /// <summary>
    /// [Admin] Admin onayını bekleyen tarifleri sayfalamalı olarak listeler.
    /// </summary>
    [HttpGet("pending-approvals")]
    [Authorize(Roles = "Admin")] // Projendeki role ismine göre (AuthRoles.Admin vs) ayarlayabilirsin
    public async Task<IActionResult> GetPendingApprovals([FromQuery] GetPendingAdminRecipesQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// [Admin] Bekleyen bir tarifi onaylar ve topluluğa açar.
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve([FromRoute] Guid id)
    {
        var command = new ApproveRecipeCommand { Id = id };
        var resultMessage = await Mediator.Send(command);
        return Ok(new { Message = resultMessage });
    }

    /// <summary>
    /// [Admin] Bekleyen bir tarifi gerekçe göstererek reddeder.
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] RejectRecipeCommand command)
    {
        command.Id = id;
        var resultMessage = await Mediator.Send(command);
        return Ok(new { Message = resultMessage });
    }
}
