using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace DBox_CS.WebAPI.Authentication
{
    public class OAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OAuthMiddleware> _logger;

        public OAuthMiddleware(RequestDelegate next, ILogger<OAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            await _next(context);

            // 3. Log the result after processing
            var user = context.User.Identity?.IsAuthenticated == true
                       ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       : "Anonymous";

            _logger.LogInformation(
                "Request {Method} {Path} processed for User: {User}. Status: {Status}",
                method, path, user, context.Response.StatusCode);
        }
    }

}
