using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthECAPI.Controllers
{
    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    public class UserLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app, IConfiguration config)
        {
            app.MapPost("/signup", CreateUser);
            app.MapPost("/signin", SigninUser);
            return app;
        }

        private static async Task<IResult> CreateUser(UserManager<AppUser> userManager, [FromBody] UserRegistrationModel userRegistrationModel)
        {
            AppUser user = new AppUser()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName
            };
            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
            if (result.Succeeded)
                return Results.Ok(result);
            else
                return Results.BadRequest(result);
        }

        private static async Task<IResult> SigninUser(UserManager<AppUser> userManager, [FromBody] UserLoginModel userLoginModel, IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(userLoginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, userLoginModel.Password))
            {
                var loginKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JwtSecret));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim("UserID", user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(
                        loginKey,
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Results.Ok(new { token });
            }
            else
            {
                return Results.BadRequest(new { message = "Username or password is incorrect" });
            }
        }
    }
}
