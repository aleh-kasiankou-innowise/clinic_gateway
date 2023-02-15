using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Innowise.Clinic.Gateway.Middleware.Exceptions;
using Innowise.Clinic.Gateway.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Innowise.Clinic.Gateway.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtValidationConfigurationOptions _jwtValidationOptions;


    public TokenValidationMiddleware(RequestDelegate next, IOptions<JwtOptions> jwtOptions,
        IOptions<JwtValidationConfigurationOptions> jwtValidation)
    {
        _next = next;
        _jwtOptions = jwtOptions.Value;
        _jwtValidationOptions = jwtValidation.Value;
    }

    // Validate token itself
    // Check if user token should be renewed by contacting Authorization service

    public async Task InvokeAsync(HttpContext context)
    {
        {
            string authHeader = context.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    // TODO Move JWT Claims and Roles to shared Nuget Package
                    
                    var principal = ValidateToken(token);
                    var userIdClaim = principal.Claims.FirstOrDefault(x => x.Type == "user-id");
                    if (userIdClaim == null || await IsUserLoginRequired(userIdClaim.Value))
                    {
                        throw new InvalidTokenException(
                            "The token is invalid. Please log in to get a new pair of tokens.");
                    }
                }
                catch (Exception e)
                {
                    await StopRequest(context, e.Message);
                    return;
                }
            }

            await _next(context);
        }
    }

    private ClaimsPrincipal ValidateToken(string token)
    {
        var jwtTokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = _jwtValidationOptions.ValidateIssuer,
            ValidIssuer = _jwtOptions.ValidIssuer,
            ValidateIssuerSigningKey = _jwtValidationOptions.ValidateIssuerSigningKey,
            ValidateAudience = _jwtValidationOptions.ValidateAudience,
            ValidAudience = _jwtOptions.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            ValidateLifetime = _jwtValidationOptions.ValidateLifetime
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ValidateToken(token, jwtTokenValidationParameters,
            out _);
    }

    private async Task<bool> IsUserLoginRequired(string userId)
    {
        var httpClient = new HttpClient();

        var response = await httpClient.GetAsync($"http://auth/validation/user-tokens/{userId}");
        if (response.IsSuccessStatusCode)
        {
            var isValidationSucceeded = await response.Content.ReadFromJsonAsync<bool>();
            if (isValidationSucceeded)
            {
                return false;
            }

            return true;
        }

        throw new AuthenticationServiceUnavailableException("The internal error. Please try again a bit later.");
    }

    private async Task StopRequest(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        var jsonMessage = JsonSerializer.Serialize(message);
        await context.Response.WriteAsync(jsonMessage, Encoding.UTF8);
    }
}