namespace StoreBackEnd.Api.Core.Exceptions
{
    // for HTTP 409(tq email deja utilise)
    public class ConflictException(string message) : Exception(message)
    {
    }
}