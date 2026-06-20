using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Admin.Queries.GetDashboardStats;

namespace OleAI.Backend.Presentation.Controllers;

[Route("api/admin/dashboard")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : BaseController
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await Mediator.Send(new GetDashboardStatsQuery());
        return Ok(result);
    }
}
