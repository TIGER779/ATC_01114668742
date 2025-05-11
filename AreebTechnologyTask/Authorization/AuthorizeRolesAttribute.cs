using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AreebTechnologyTask.Authorization
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get JWT token from cookie
            var token = context.HttpContext.Request.Cookies["JwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Auth",
                    new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            try
            {
                // Parse and validate JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Auth",
                        new { returnUrl = context.HttpContext.Request.Path });
                    return;
                }

                // Get role claim from JWT
                var userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userRole))
                {
                    context.Result = new RedirectToActionResult("Login", "Auth",
                        new { returnUrl = context.HttpContext.Request.Path });
                    return;
                }

                // Check if user has the required role
                if (!_roles.Contains(userRole))
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "_Unauthorized",
                        ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                            new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                            new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
                        {
                            { "UserRole", userRole }
                        }
                    };
                    return;
                }
            }
            catch
            {
                // If token is invalid or expired
                context.Result = new RedirectToActionResult("Login", "Auth",
                    new { returnUrl = context.HttpContext.Request.Path });
                return;
            }
        }
    }
}