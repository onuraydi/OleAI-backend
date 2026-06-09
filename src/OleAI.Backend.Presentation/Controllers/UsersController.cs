using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile;
using OleAI.Backend.Application.Features.Users.Queries.GetAdminUsers;
using OleAI.Backend.Application.Features.Users.Queries.GetRecommendation;
using OleAI.Backend.Application.Features.Users.Queries.GetUserProfile;

namespace OleAI.Backend.Presentation.Controllers
{
    /// <summary>
    /// Yetkilendirilmiş kullanıcının profil, beslenme tercihi ve alerji bilgilerini yöneten API endpoint'leri.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Giriş yapan kullanıcının profil bilgilerini, beslenme tercihlerini ve alerjilerini getirir.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await Mediator.Send(new GetUserProfileQuery());
            return Ok(result);
        }

        /// <summary>
        /// Giriş yapan kullanıcının temel profil bilgilerini (Ad, Soyad) günceller.
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserProfileCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Giriş yapan kullanıcının beslenme tercihlerini günceller (Upsert).
        /// Eski tercihler temizlenip yalnızca gönderilen liste aktif edilir.
        /// </summary>
        [HttpPut("me/dietary-preferences")]
        public async Task<IActionResult> UpdateDietaryPreferences([FromBody] UpdateUserPreferencesCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Giriş yapan kullanıcının alerji ve gıda kısıtlarını günceller (Upsert).
        /// Eski kısıtlar temizlenip yalnızca gönderilen liste aktif edilir.
        /// </summary>
        [HttpPut("me/allergies")]
        public async Task<IActionResult> UpdateAllergies([FromBody] UpdateUserAllergiesCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Giriş yapan kullanıcının sevmediği malzemeleri günceller (Upsert).
        /// Eski kayıtlar temizlenip yalnızca gönderilen liste aktif edilir.
        /// </summary>
        [HttpPut("me/disliked-ingredients")]
        public async Task<IActionResult> UpdateDislikedIngredients([FromBody] OleAI.Backend.Application.Features.Users.Commands.UpdateUserDislikedIngredients.UpdateUserDislikedIngredientsCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }

        /// <summary>
        /// Kullanıcının diyet tercihleri ve alerjilerine göre AI destekli kişiselleştirilmiş tarif önerileri alır.
        /// </summary>
        [HttpGet("me/recommendations")]
        public async Task<IActionResult> GetRecommendations([FromQuery] GetRecommendationQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Sadece Admin rolüne sahip kullanıcıların tüm kullanıcı listesini (rolleri, kilit durumu vs.) sayfalamalı olarak getirmesini sağlar.
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = AuthRoles.Admin)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await Mediator.Send(new GetAdminUsersQuery { PageNumber = page, PageSize = size });
            return Ok(result);
        }
    }
}
