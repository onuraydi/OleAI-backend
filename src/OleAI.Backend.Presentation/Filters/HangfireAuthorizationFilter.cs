using Hangfire.Dashboard;

namespace OleAI.Backend.Presentation.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Sadece yetkili kullanıcıların erişimine izin ver (Örn: Admin rolü)
            // JWT token çerezde veya header'da varsa User.Identity.IsAuthenticated true olur.
            return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole("Admin");
        }
    }
}
