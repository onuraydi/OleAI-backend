using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;

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
}
