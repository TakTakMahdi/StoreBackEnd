using StoreBackEnd.Api.Core.Entities;

namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface ITokenService
    {
        string GenererAccessToken(Utilisateur utilisateur);

        string GenererRefreshToken();

        // validates an expired access token's signature to read its claims during refresh
        System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}