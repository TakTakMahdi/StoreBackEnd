using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Enums;

namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface ICommandeRepository : IGenericRepository<Commande>
    {
        Task<List<Commande>> GetCommandesParLivreurAsync(int livreurId, StatutCommande? statut = null);

        Task<List<Commande>> GetHistoriqueClientAsync(int clientId);
    }
}