using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetCategories;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetTags;

namespace OleAI.Backend.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipeTaxonomyController : BaseController
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await Mediator.Send(new GetRecipeCategoriesQuery()));
    }

    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory([FromBody] CreateRecipeCategoryCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPut("categories")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateRecipeCategoryCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        return Ok(await Mediator.Send(new DeleteRecipeCategoryCommand { Id = id }));
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetTags()
    {
        return Ok(await Mediator.Send(new GetTagsQuery()));
    }

    [HttpPost("tags")]
    public async Task<IActionResult> AddTag([FromBody] CreateTagCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPut("tags")]
    public async Task<IActionResult> UpdateTag([FromBody] UpdateTagCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpDelete("tags/{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        return Ok(await Mediator.Send(new DeleteTagCommand { Id = id }));
    }
}
