using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBox_CS.WebAPI.Authentication
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _config;

        public ApiKeyAuthFilter(IConfiguration config)
        {
            _config = config;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractApiKey))
            {
                context.Result=new UnauthorizedObjectResult("API key Missing");
                return;
            }
            var apikey = _config.GetValue<string>(AuthConstants.ApiKeySectionName);

            if (!apikey.Equals(extractApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key");
                return;
            }

            

        }
    }
}
