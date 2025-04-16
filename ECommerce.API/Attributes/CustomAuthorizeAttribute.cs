using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private const string ExpectedToken = "FAKE-TOKEN-123";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasHeader = context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token);
            if (!hasHeader || token != ExpectedToken)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Unauthorized request" });
            }
        }
    }
}
