using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Pantry.Commands.AddMultipleUserIngredients;
using OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Commands.CancelRequest;
using OleAI.Backend.Application.Features.Pantry.Commands.ClearExpiredUserIngredients;
using OleAI.Backend.Application.Features.Pantry.Commands.DeleteUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Commands.UpdateUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Queries.GetCategorizedPopularIngredients;
using OleAI.Backend.Application.Features.Pantry.Queries.GetMyPantry;
using OleAI.Backend.Application.Features.Pantry.Queries.SearchIngredient;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PantryController : BaseController
    {
        private Guid LoggedInUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı."));

        /// <summary>
        /// Kullanıcının dolabındaki malzemeleri getirir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyPantry()
        {
            var query = new GetMyPantryQuery { UserId = LoggedInUserId };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Redis destekli auto-complete malzeme araması yapar.
        /// </summary>
        /// <param name="keyword">Aranacak kelime (en az 2 harf)</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchIngredients([FromQuery] string keyword)
        {
            var query = new SearchIngredientQuery { Keyword = keyword };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Kutuplaştırıcı veya topluluk tarafından en çok sevilmeyen popüler malzemeleri getirir.
        /// </summary>
        [HttpGet("popular-dislikes")]
        public async Task<IActionResult> GetPopularDislikedIngredients()
        {
            var query = new OleAI.Backend.Application.Features.Ingredients.Queries.GetPopularDislikedIngredients.GetPopularDislikedIngredientsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Kategori bazında popüler malzemeleri getirir.
        /// </summary>
        [HttpGet("popular-by-category")]
        public async Task<IActionResult> GetPopularIngredientsByCategory()
        {
            var query = new GetCategorizedPopularIngredientsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Dolaba yeni bir malzeme ekler. Sistemde yoksa otomatik olarak onaya (LLM) gönderir.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddIngredient([FromBody] AddUserIngredientCommand command)
        {
            // GÜVENLİK: Dışarıdan gelen UserId'yi eziyoruz ve token'daki gerçek ID'yi atıyoruz.
            command.Request.UserId = LoggedInUserId;

            var result = await Mediator.Send(command);

            return Created("", result);
        }

        /// <summary>
        /// Dolaba birden fazla malzemeyi aynı anda ekler.
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> AddMultipleIngredients([FromBody] AddMultipleUserIngredientsCommand command)
        {
            // GÜVENLİK: Her bir istekteki UserId'yi eziyoruz
            foreach (var req in command.Request.Ingredients)
            {
                req.UserId = LoggedInUserId;
            }

            var result = await Mediator.Send(command);

            return Created("", result);
        }

        /// <summary>
        /// Dolaptaki bir ürünün miktarını manuel olarak günceller.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredientQuantity([FromRoute] Guid id, [FromBody] UpdateUserIngredientRequest request)
        {
            request.Id = id;
            request.UserId = LoggedInUserId;

            var command = new UpdateUserIngredientCommand
            {
                Request = request,
            };

            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Dolaptaki bir ürünü manuel olarak siler.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient([FromRoute] Guid id)
        {
            var command = new DeleteUserIngredientCommand
            {
                Id = id,
                UserId = LoggedInUserId
            };
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Kullanıcının dolabında son kullanma tarihi geçmiş tüm ürünleri otomatik temizler.
        /// </summary>
        [HttpDelete("clear-expired")]
        public async Task<IActionResult> ClearExpiredIngredients()
        {
            var command = new ClearExpiredUserIngredientsCommand
            {
                userId = LoggedInUserId
            };
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Kullanıcının henüz onay bekleyen (Pending) bir malzeme talebini iptal etmesini sağlar.
        /// </summary>
        [HttpDelete("requests/{requestId}")]
        public async Task<IActionResult> CancelPendingRequest([FromRoute] Guid requestId)
        {
            var command = new CancelIngredientRequestCommand
            {
                RequestId = requestId,
                UserId = LoggedInUserId
            };
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }
    }
}
