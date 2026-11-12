namespace StoreBackEnd.Api.Core.Exceptions
{
    // for HTTP 404
    public class NotFoundException(string entite, object id) : Exception($"{entite} avec l'id '{id}' introuvable.")
    {
    }
}