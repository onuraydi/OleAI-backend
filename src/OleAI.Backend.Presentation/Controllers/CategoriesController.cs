using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.CreateCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.DeleteCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.UpdateCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryById;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryList;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetCategoryListQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetCategoryByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCategoryCommand command)
        {
            var result = await Mediator.Send(command);
            return Created("", new { Id = result, Message = "Kategori eklendi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryCommand command)
        {
            command.Request.Id = id;
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new DeleteCategoryCommand { Id = id });
            return Ok(new { Message = result });
        }
    }
}
