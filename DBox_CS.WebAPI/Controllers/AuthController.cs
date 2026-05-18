using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace DBox_CS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) => _config = config;

        [HttpPost("token")]
        [Consumes("application/x-www-form-urlencoded")] // OAuth2 standard
        public IActionResult GetToken([FromForm] string client_id, [FromForm] string client_secret)
        {
            //// 1. Validate Grant Type
            //if (grant_type != "client_credentials")
            //    return BadRequest(new { error = "unsupported_grant_type" });

            // 2. Validate Client ID & Secret against appsettings (or DB)
            var validId = _config["ClientCredentials:ClientId"];
            var validSecret = _config["ClientCredentials:ClientSecret"];

            if (client_id != validId || client_secret != validSecret)
                return Unauthorized(new { error = "invalid_client" });

            // 3. Generate Token
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("client_id", validId) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return standard OAuth2 response
            return Ok(new
            {
                access_token = tokenHandler.WriteToken(token),
                token_type = "Bearer",
                expires_in = 3600
            });
        }
        // Note: Use the EXACT same key you put in Startup.cs
        private const string SecretKey = "DarwinBoxIntegrationCivilSoftKey2026";

        //[HttpPost("login")]
        //public IActionResult Login([FromBody] LoginModel model)
        //{
        //    // 1. Validate the user (Replace this with your actual DB check)
        //    if (model.Username == "cpiuser" && model.Password == "Cpi@12345")
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes(SecretKey);

        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(new[]
        //            {
        //                new Claim(ClaimTypes.Name, model.Username),
        //                new Claim("Role", "Admin") // You can add custom claims here
        //            }),
        //            Expires = DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
        //            SigningCredentials = new SigningCredentials(
        //                new SymmetricSecurityKey(key),
        //                SecurityAlgorithms.HmacSha256Signature)
        //        };

        //        var token = tokenHandler.CreateToken(tokenDescriptor);
        //        var tokenString = tokenHandler.WriteToken(token);

        //        return Ok(new { Token = tokenString });
        //    }

        //    return Unauthorized("Invalid credentials");
        //}
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}