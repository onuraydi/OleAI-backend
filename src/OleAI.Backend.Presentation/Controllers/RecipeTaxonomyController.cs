using Microsoft.AspNetCore.Mvc;
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
    [HttpGet("tags")]
    public async Task<IActionResult> GetTags()
    {
        return Ok(await Mediator.Send(new GetTagsQuery()));
    }
}
