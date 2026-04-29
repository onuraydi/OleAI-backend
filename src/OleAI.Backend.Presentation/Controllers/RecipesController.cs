using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.ChangeRecipeVisibility;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.RejectRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
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

    /// <summary>
    /// Kullanıcının kendisine ait bir tarifi (görseller, adımlar ve malzemeler dahil) günceller.
    /// </summary>
    /// <remarks>
    /// Görsel (IFormFile) içerdiği için bu endpoint multipart/form-data kabul eder.
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize] // Sadece giriş yapmış kullanıcılar kendi tarifini güncelleyebilir
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateRecipeCommand updateRecipeCommand)
    {
        // URL'den gelen ID'yi Command nesnesine atıyoruz
        updateRecipeCommand.Request.Id = id;

        string resultMessage = await Mediator.Send(updateRecipeCommand);

        return Ok(new { Message = resultMessage });
    }

    /// <summary>
    /// Kullanıcının kendisine ait bir tarifi sistemden kalıcı olarak siler.
    /// Cloudinary üzerindeki görseller de otomatik olarak temizlenir.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize] // Sadece giriş yapmış kullanıcılar kendi tarifini silebilir
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleteRecipeCommand = new DeleteRecipeCommand { Id = id };

        string resultMessage = await Mediator.Send(deleteRecipeCommand);

        return Ok(new { Message = resultMessage });
    }

    /// <summary>
    /// Kullanıcının kendisine ait bir tarifin görünürlük (Public/Private) durumunu değiştirir.
    /// </summary>
    [HttpPatch("{id}/visibility")]
    [Authorize]
    public async Task<IActionResult> ChangeVisibility([FromRoute] Guid id, [FromBody] ChangeRecipeVisibilityCommand command)
    {
        command.Id = id;

        string resultMessage = await Mediator.Send(command);

        return Ok(new { Message = resultMessage });
    }
}
