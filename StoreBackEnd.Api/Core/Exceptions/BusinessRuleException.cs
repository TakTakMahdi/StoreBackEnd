namespace StoreBackEnd.Api.Core.Exceptions
{
    // tq stock insuffisant,
    public class BusinessRuleException(string message) : Exception(message)
    {
    }
}