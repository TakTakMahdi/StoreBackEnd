namespace StoreBackEnd.Api.Core.Exceptions
{
    // for HTTP 401
    public class UnauthorizedException(string message) : Exception(message)
    {
    }
}