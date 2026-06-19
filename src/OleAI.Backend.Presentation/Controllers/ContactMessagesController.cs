using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactMessagesController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateContactMessageCommand createContactMessageCommand)
        {
            var result = await Mediator.Send(createContactMessageCommand);
            return Created("", result);
        }
    }
}
