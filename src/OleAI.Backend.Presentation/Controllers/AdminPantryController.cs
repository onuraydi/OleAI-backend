using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/admin/pantry-requests")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPantryController : BaseController
    {
    }
}
