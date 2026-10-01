using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CapacitaGRD_Admin.Controllers
{
    public class ValidateSessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var authToken = session.GetString("AuthToken");

            if (string.IsNullOrEmpty(authToken) ||
                !DateTime.TryParse(session.GetString("AuthTokenExpiration"), out DateTime expiration) ||
                expiration <= DateTime.UtcNow)
            {
                context.Result = new RedirectToActionResult("LoginReturn", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
