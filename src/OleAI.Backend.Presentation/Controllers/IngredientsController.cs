using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.CreateIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.DeleteIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetAliasesByIngredientId;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasList;
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

        // ALİAS

        /// <summary>
        /// Belirli bir malzemeye ait eşanlamlı kelimeleri (Alias) getirir.
        /// </summary>
        [HttpGet("{ingredientId}/aliases")]
        public async Task<IActionResult> GetAliases([FromRoute] Guid ingredientId)
        {
            var result = await Mediator.Send(new GetAliasesByIngredientIdQuery { IngredientId = ingredientId });
            return Ok(result);
        }

        /// <summary>
        /// Tüm alias'ları getirir.
        /// </summary>
        [HttpGet("aliases")]
        public async Task<IActionResult> GetList([FromQuery] GetIngredientAliasListQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Alias'ın ID değerine göre getir.
        /// </summary>
        [HttpGet("aliases/{id}")]
        public async Task<IActionResult> GetByIdAlias(Guid id)
        {
            var query = new GetIngredientByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Bir malzemeye manuel olarak yeni bir eşanlamlı kelime ekler.
        /// </summary>
        [HttpPost("{ingredientId}/aliases")]
        public async Task<IActionResult> AddAlias([FromRoute] Guid ingredientId, [FromBody] string aliasName)
        {
            var request = new CreateIngredientAliasRequest
            {
                IngredientId = ingredientId,
                AliasName = aliasName
            };

            var command = new CreateIngredientAliasCommand
            {
                Request = request,
            };

            var result = await Mediator.Send(command);
            return Created("", new { Id = result, Message = "Eşanlamlı kelime eklendi." });
        }

        /// <summary>
        /// Bir eşanlamlı kelimeyi tamamen siler.
        /// </summary>
        [HttpDelete("aliases/{aliasId}")]
        public async Task<IActionResult> DeleteAlias([FromRoute] Guid aliasId)
        {
            // Not: Silme işlemi doğrudan aliasId ile yapılabildiği için rotayı {ingredientId}'den ayırdık.
            var result = await Mediator.Send(new DeleteIngredientAliasCommand { Id = aliasId });
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Silinmiş bir eşanlamlı kelimeyi geri getirir.
        /// </summary>
        [HttpPatch("aliases/{aliasId}/restore")]
        public async Task<IActionResult> RestoreAlias([FromRoute] Guid aliasId)
        {
            var result = await Mediator.Send(new OleAI.Backend.Application.Features.IngredientAliases.Commands.RestoreIngredientAlias.RestoreIngredientAliasCommand { Id = aliasId });
            return Ok(new { Message = result });
        }
    }
}
