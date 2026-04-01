using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Ingredients.Commands.CreateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Commands.DeleteIngredient;
using OleAI.Backend.Application.Features.Ingredients.Commands.UpdateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientById;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientList;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class IngredientsController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetIngredientListQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateIngredientCommand command)
        {
            var result = await Mediator.Send(command);
            return Created("", new { Id = result, Message = "Malzeme başarıyla eklendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new DeleteIngredientCommand { Id = id });
            return Ok(new { Message = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateIngredientCommand command)
        {
            command.Request.Id = id;
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetIngredientByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
