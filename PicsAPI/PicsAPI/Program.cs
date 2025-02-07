
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAppController.DataLoaders;
using WebAppController.Interfaces;
using WebAppController.Middlewares;

namespace WebAppController
{
    public class Program() {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IDataLoader, FileDataLoader>();
            //builder.Services.AddSingleton<IDataLoader, MongoDataLoader>();

            builder.Services.AddCors(
                options => {
                    options.AddPolicy(
                        "AngularClient",
                        builder => builder
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                    );
                }
            );

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options => {
                    var tokenIssuer = builder.Configuration["Jwt:Issuer"];
                    var tokenAudience = builder.Configuration["Jwt:Audience"];
                    var tokenKey = builder.Configuration["Jwt:Key"];
                    if (tokenIssuer == null || tokenAudience == null || tokenKey == null) {
                        throw new ArgumentNullException("All token configs not found.");
                    }

                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = tokenIssuer,
                        ValidAudience = tokenAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey))
                    };

                    // Extract JWT token from cookies
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            var cookieHeader = context.Request.Headers["Cookie"].ToString();
                            if (!string.IsNullOrEmpty(cookieHeader)) {
                                var jwtToken = cookieHeader
                                    .Split(';')
                                    .Select(cookie => cookie.Trim())
                                    .FirstOrDefault(cookie => {
                                        return cookie.StartsWith("jwt=", StringComparison.OrdinalIgnoreCase);
                                    });
                                if (jwtToken != null) {
                                    context.Request.Headers.Authorization = "Bearer " + jwtToken.Substring(4);
                                    Console.WriteLine($"token: {context.Token}");
                                }
                            }
                            return Task.CompletedTask;
                        }
                    };
                }
            );
            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseCors("AngularClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<MyLogger>();

            app.MapControllers();

            app.Run();
        }
    }
}
