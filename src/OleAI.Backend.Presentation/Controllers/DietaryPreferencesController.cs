using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.CreateDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.DeleteDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.RestoreDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Commands.UpdateDietaryPreference;
using OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceById;
using OleAI.Backend.Application.Features.DietaryPreferences.Queries.GetDietaryPreferenceList;

namespace OleAI.Backend.Presentation.Controllers;

[Route("api/dietary-preferences")]
[ApiController]
public class DietaryPreferencesController : BaseController
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetList()
    {
        var result = await Mediator.Send(new GetDietaryPreferenceListQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new GetDietaryPreferenceByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add([FromBody] CreateDietaryPreferenceCommand command)
    {
        var result = await Mediator.Send(command);
        return Created("", new { Message = result });
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateDietaryPreferenceCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(new { Message = result });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new DeleteDietaryPreferenceCommand { Id = id });
        return Ok(new { Message = result });
    }

    [HttpPut("{id}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Restore([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new RestoreDietaryPreferenceCommand { Id = id });
        return Ok(new { Message = result });
    }
}
