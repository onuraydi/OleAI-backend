using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Pantry.Commands.ResolveRequest;
using OleAI.Backend.Application.Features.Pantry.Queries.GetPendingRequests;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/admin/pantry-requests")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPantryController : BaseController
    {
        /// <summary>
        /// Yapay zeka (LLM) analizi tamamlanmış ve onay bekleyen yeni malzeme taleplerini listeler.
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var query = new GetPendingIngredientRequestsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Bekleyen bir talebi karara bağlar (Yeni Ekle, Alias Olarak Birleştir veya Reddet).
        /// </summary>
        [HttpPost("resolve")]
        public async Task<IActionResult> ResolveRequest([FromBody] ResolveIngredientRequestCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }
    }
}
