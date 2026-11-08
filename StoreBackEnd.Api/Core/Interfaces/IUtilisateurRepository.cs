using StoreBackEnd.Api.Core.Entities;

namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface IUtilisateurRepository : IGenericRepository<Utilisateur>
    {
        Task<Utilisateur?> GetByEmailAsync(string email);
    }
}