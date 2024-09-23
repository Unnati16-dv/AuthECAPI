using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthECAPI.Extensions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection InjectDBContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("SqlConnection")));
            return services;
        }
        public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                x.DefaultChallengeScheme =
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(y =>
            {
                y.SaveToken = false;
                y.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["AppSettings:JwtSecret"]!))
                };
            });
            return services;
        }

    }
}
