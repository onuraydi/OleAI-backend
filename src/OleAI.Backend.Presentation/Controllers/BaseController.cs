using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OleAI.Backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private IMediator? _mediator;

        // Sadece miras alan sınıfların erişebileceği (protected) bir özellik tanımlıyoruz.
        // Eğer _mediator null ise, IoC Container'dan (RequestServices) çekip atıyor (Lazy Loading).
        // Bu sayede Child sınıflarda Constructor yazma zorunluluğunu tamamen ortadan kaldırıyoruz!
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
    }
}

