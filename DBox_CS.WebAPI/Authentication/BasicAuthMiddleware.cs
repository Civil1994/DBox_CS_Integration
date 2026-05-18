using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace DBox_CS.WebAPI.Authentication
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string Username = "cpiuser";
        private const string Password = "Cpi@12345";

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            if (path.Contains("dboxcsi"))  // ← Flexible match!
            {
                Console.WriteLine($"BasicAuth HIT: {context.Request.Path}");  // Debug

                if (!ValidateBasicAuth(context))
                {
                    Console.WriteLine(" Auth FAILED");  
                    context.Response.StatusCode = 401;
                    context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"DboxCSI\"");
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                Console.WriteLine(" Auth PASSED");  
            }

            await _next(context);
        }

        private bool ValidateBasicAuth(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
                return false;

            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!authHeader.StartsWith("Basic "))
                return false;

            try
            {
                var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var parts = credentials.Split(':', 2);
                return parts.Length == 2 && parts[0] == Username && parts[1] == Password;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}
