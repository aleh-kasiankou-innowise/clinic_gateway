namespace Innowise.Clinic.Gateway.Options;

public class JwtOptions
{
    public string Key { get; set; }
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
}