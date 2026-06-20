using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Notifications.Commands.RegisterDevice;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserDevicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserDevicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
