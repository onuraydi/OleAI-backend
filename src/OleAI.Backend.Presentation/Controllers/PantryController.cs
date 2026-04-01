using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient;
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
    }
}
