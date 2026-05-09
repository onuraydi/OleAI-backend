using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.DeleteAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Commands.UpdateAllergyRestriction;
using OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionById;
using OleAI.Backend.Application.Features.AllergyRestrictions.Queries.GetAllergyRestrictionList;

namespace OleAI.Backend.Presentation.Controllers;

[Route("api/allergy-restrictions")]
[ApiController]
public class AllergyRestrictionsController : BaseController
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetList()
    {
        var result = await Mediator.Send(new GetAllergyRestrictionListQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new GetAllergyRestrictionByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add([FromBody] CreateAllergyRestrictionCommand command)
    {
        var result = await Mediator.Send(command);
        return Created("", new { Message = result });
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateAllergyRestrictionCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(new { Message = result });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new DeleteAllergyRestrictionCommand { Id = id });
        return Ok(new { Message = result });
    }
}
