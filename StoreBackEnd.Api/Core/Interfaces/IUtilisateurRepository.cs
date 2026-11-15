using StoreBackEnd.Api.Core.Entities;

namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface IUtilisateurRepository : IGenericRepository<Utilisateur>
    {
        Task<Utilisateur?> GetByEmailAsync(string email);
        Task<Utilisateur?> GetByIdWithRefreshTokenAsync(int utilisateurId, string refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    }
}