using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Recipes.Commands.AddComment;
using OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.ChangeRecipeVisibility;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteComment;
using OleAI.Backend.Application.Features.Recipes.Commands.DeleteRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.RejectRecipe;
using OleAI.Backend.Application.Features.Recipes.Commands.StartCooking;
using OleAI.Backend.Application.Features.Recipes.Commands.SyncAllToAi;
using OleAI.Backend.Application.Features.Recipes.Commands.ToggleInteraction;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateComment;
using OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyDislikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyLikedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMySavedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPendingAdminRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetPublishedRecipes;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipesForAi;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Presentation.ModelBinders;

namespace OleAI.Backend.Presentation.Controllers;

/// <summary>
/// Tarif yönetimi ve etkileşim işlemleri için API endpoints'ini sağlar.
/// </summary>
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
    /// IngredientsJson ve StepsJson alanları JSON array olmalıdır.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Add([ModelBinder(BinderType = typeof(CreateRecipeCommandModelBinder))] CreateRecipeCommand createRecipeCommand)
    {
        string resultMessage = await Mediator.Send(createRecipeCommand);

        return Created(uri: "", new { Message = resultMessage });
    }

    /// <summary>
    /// [Admin] Admin onayını bekleyen tarifleri sayfalamalı olarak listeler.
    /// </summary>
    [HttpGet("pending-approvals")]
    [Authorize(Roles = "Admin")]
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
    /// [Admin] Veritabanındaki tüm onaylanmış tarifleri AI motoruna senkronize eder.
    /// Genellikle ilk kurulumda veya veri tutarsızlığı durumunda manuel olarak kullanılır.
    /// </summary>
    [HttpPost("sync-all-to-ai")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SyncAllToAi()
    {
        var resultMessage = await Mediator.Send(new SyncAllToAiCommand());
        return Ok(new { Message = resultMessage });
    }

    /// <summary>
    /// Kullanıcının kendisine ait bir tarifi (görseller, adımlar ve malzemeler dahil) günceller.
    /// </summary>
    /// <remarks>
    /// Görsel (IFormFile) içerdiği için bu endpoint multipart/form-data kabul eder.
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] Guid id, [ModelBinder(BinderType = typeof(UpdateRecipeCommandModelBinder))] UpdateRecipeCommand updateRecipeCommand)
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
    [Authorize]
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

    /// <summary>
    /// Topluluğa açık, onaylanmış tarifleri sayfalamalı ve aranabilir şekilde listeler.
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPublishedRecipes([FromQuery] GetPublishedRecipesQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının kendi girdiği tüm tarifleri listeler.
    /// </summary>
    [HttpGet("my-recipes")]
    [Authorize]
    public async Task<IActionResult> GetMyRecipes([FromQuery] GetMyRecipesQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Bir tarifin tüm detaylarını (malzemeler, adımlar, resimler, etkileşimler) getirir.
    /// Gizli tarifleri sadece tarifin sahibi görebilir.
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous] // İçerideki kurala göre engellenir veya izin verilir
    public async Task<IActionResult> GetDetail([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new GetRecipeDetailQuery { Id = id });
        return Ok(result);
    }

    /// <summary>
    /// AI motorunun başlangıçta tüm tarifleri çekmesi için kullanılan özel endpoint.
    /// </summary>
    [HttpGet("ai-sync")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForAi()
    {
        var result = await Mediator.Send(new GetRecipesForAiQuery());
        return Ok(result);
    }

    /// <summary>
    /// Tarifi pişirme işlemini başlatır ve malzemeleri stoktan düşer.
    /// </summary>
    [HttpPost("{id}/start-cooking")]
    [Authorize]
    public async Task<IActionResult> StartCooking([FromRoute] Guid id)
    {
        var resultMessage = await Mediator.Send(new StartCookingCommand { RecipeId = id });
        return Ok(new { Message = resultMessage });
    }

    /// <summary>
    /// Tarife beğeni (Like) atar veya mevcut beğeniyi kaldırır.
    /// </summary>
    [HttpPost("{id}/like")]
    [Authorize]
    public async Task<IActionResult> ToggleLike([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new ToggleInteractionCommand { RecipeId = id, InteractionType = "Like" });
        return Ok(new { Message = result });
    }

    /// <summary>
    /// Tarifi beğenmeme (Dislike) olarak işaretler veya kaldırır.
    /// </summary>
    [HttpPost("{id}/dislike")]
    [Authorize]
    public async Task<IActionResult> ToggleDislike([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new ToggleInteractionCommand { RecipeId = id, InteractionType = "Dislike" });
        return Ok(new { Message = result });
    }

    /// <summary>
    /// Tarifi kullanıcının favorilerine (Saved) ekler veya çıkarır.
    /// </summary>
    [HttpPost("{id}/save")]
    [Authorize]
    public async Task<IActionResult> ToggleSave([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new ToggleInteractionCommand { RecipeId = id, InteractionType = "Save" });
        return Ok(new { Message = result });
    }

    /// <summary>
    /// Tarife ana yorum veya başka bir yoruma cevap ekler.
    /// </summary>
    [HttpPost("{id}/comments")]
    [Authorize]
    public async Task<IActionResult> AddComment([FromRoute] Guid id, [FromBody] AddCommentCommand command)
    {
        command.RecipeId = id;
        var result = await Mediator.Send(command);
        return Ok(new { Message = result });
    }

    /// <summary>
    /// Kullanıcının kendi yorumunu günceller.
    /// </summary>
    [HttpPut("comments/{commentId}")]
    [Authorize]
    public async Task<IActionResult> UpdateComment([FromRoute] Guid commentId, [FromBody] UpdateCommentCommand command)
    {
        command.CommentId = commentId;
        return Ok(new { Message = await Mediator.Send(command) });
    }

    /// <summary>
    /// Kullanıcının kendi yorumunu siler.
    /// </summary>
    [HttpDelete("comments/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId)
    {
        return Ok(new { Message = await Mediator.Send(new DeleteCommentCommand { CommentId = commentId }) });
    }


    // TODO BUNLAR DAHA SONRA USERSCONTROLLER'A YAZILACAK

    /// <summary>
    /// Giriş yapan kullanıcının "Favorilerime Kaydettiği" tarifleri sayfalamalı getirir.
    /// </summary>
    [HttpGet("me/saved-recipes")]
    [Authorize]
    public async Task<IActionResult> GetMySavedRecipes([FromQuery] GetMySavedRecipesQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Giriş yapan kullanıcının "Beğendiği" tarifleri sayfalamalı getirir.
    /// </summary>
    [HttpGet("me/liked-recipes")]
    [Authorize]
    public async Task<IActionResult> GetMyLikedRecipes([FromQuery] GetMyLikedRecipesQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Giriş yapan kullanıcının "Beğenmediği" tarifleri sayfalamalı getirir.
    /// </summary>
    [HttpGet("me/disliked-recipes")]
    [Authorize]
    public async Task<IActionResult> GetMyDisikedRecipes([FromQuery] GetMyDislikedRecipesQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
