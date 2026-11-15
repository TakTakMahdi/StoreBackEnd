namespace StoreBackEnd.Api.Core.Exceptions
{
    // for HTTP 404
    public class NotFoundException(string message) : Exception(message)
    {
        public NotFoundException(string entite, object id)
            : this($"{entite} avec l'id '{id}' introuvable.")
        {
        }
    }
}