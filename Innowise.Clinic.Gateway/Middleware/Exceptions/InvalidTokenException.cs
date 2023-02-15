namespace Innowise.Clinic.Gateway.Middleware.Exceptions;

public class InvalidTokenException : ApplicationException
{
    public InvalidTokenException(string message) : base(message)
    {
        
    }
}