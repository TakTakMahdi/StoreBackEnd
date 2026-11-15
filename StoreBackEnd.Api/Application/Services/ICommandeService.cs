using StoreBackEnd.Api.Application.Dto.Commande;
using StoreBackEnd.Api.Core.Enums;

namespace StoreBackEnd.Api.Application.Services
{
    public interface ICommandeService
    {
        Task<CommandeDTO> CreerAsync(int clientId, CommandeCreateDTO dto);
        Task<List<CommandeDTO>> GetHistoriqueClientAsync(int clientId);
        Task<CommandeDTO> GetByIdAsync(int id);
        Task<List<CommandeDTO>> GetCommandesParLivreurAsync(int livreurId, StatutCommande? statut);
        Task<CommandeDTO> ConfirmerLivraisonAsync(int commandeId, int livreurId);
        Task<CommandeDTO> RejeterLivraisonAsync(int commandeId, int livreurId, CommandeRejeterDTO dto);
        Task<CommandeDTO> AssignerLivreurAsync(int commandeId, int livreurId);
    }
}