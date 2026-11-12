using Microsoft.EntityFrameworkCore;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Enums;
using StoreBackEnd.Api.Core.Interfaces;
using StoreBackEnd.Api.Infrastructure.Data;

namespace StoreBackEnd.Api.Infrastructure.Repositories
{
    public class CommandeRepository : GenericRepository<Commande>, ICommandeRepository
    {
        public CommandeRepository(ApplicationContext context) : base(context) { }

        public async Task<List<Commande>> GetCommandesParLivreurAsync(int livreurId, StatutCommande? statut = null)
        {
            var query = _dbSet
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                    .ThenInclude(l => l.Produit)
                .Where(c => c.LivreurID == livreurId);

            if (statut.HasValue)
                query = query.Where(c => c.Statut == statut.Value);

            return await query.OrderBy(c => c.DateCommande).ToListAsync();
        }

        public async Task<List<Commande>> GetHistoriqueClientAsync(int clientId) =>
            await _dbSet
                .Include(c => c.LignesCommande)
                    .ThenInclude(l => l.Produit)
                .Where(c => c.ClientID == clientId)
                .OrderByDescending(c => c.DateCommande)
                .ToListAsync();
    }
}