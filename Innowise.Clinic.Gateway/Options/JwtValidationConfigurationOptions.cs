namespace Innowise.Clinic.Gateway.Options;

public class JwtValidationConfigurationOptions
{
    public bool ValidateIssuer { get; set; }

    public bool ValidateIssuerSigningKey { get; set; }

    public bool ValidateAudience { get; set; }

    public bool ValidateLifetime { get; set; }

}