using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.ContactMessages.Commands.DeleteContactMessage;
using OleAI.Backend.Application.Features.ContactMessages.Commands.MarkMessageAsRead;
using OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/admin/contact-messages")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminContactMessagesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var query = new GetContactMessagesQuery { Page = page, Size = size };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
        {
            var command = new MarkContactMessageAsReadCommand { Id = id };
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteContactMessageCommand { Id = id };
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
