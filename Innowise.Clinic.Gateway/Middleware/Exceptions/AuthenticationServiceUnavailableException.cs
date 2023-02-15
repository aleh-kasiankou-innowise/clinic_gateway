namespace Innowise.Clinic.Gateway.Middleware.Exceptions;

public class AuthenticationServiceUnavailableException : ApplicationException
{
    public AuthenticationServiceUnavailableException(string message) : base(message)
    {
        
    }
}