using System.Text;
using Innowise.Clinic.Gateway.Middleware;
using Innowise.Clinic.Gateway.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;

namespace Innowise.Clinic.Gateway.Configuration;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureJwtAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = Convert.ToBoolean(builder.Configuration["JwtValidationConfiguration:ValidateIssuer"]),
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                ValidateAudience = Convert.ToBoolean(builder.Configuration["JwtValidationConfiguration:ValidateAudience"]),
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidateIssuerSigningKey =
                    Convert.ToBoolean(builder.Configuration["JwtValidationConfiguration:ValidateIssuerSigningKey"]),
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            };
        });
        
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));
        builder.Services.Configure<JwtValidationConfigurationOptions>(
            builder.Configuration.GetSection("JwtValidationConfiguration"));
        builder.Services.AddSingleton<TokenValidationMiddleware>();

        return builder;
    }

    public static WebApplicationBuilder ConfigureGateway(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.AddOcelot();
        builder.Configuration.AddJsonFile("ocelot.json");
        builder.Services.AddSwaggerForOcelot(builder.Configuration);
        return builder;
    }
}