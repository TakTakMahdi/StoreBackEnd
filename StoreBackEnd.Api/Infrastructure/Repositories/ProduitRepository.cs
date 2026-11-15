using Microsoft.EntityFrameworkCore;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Interfaces;
using StoreBackEnd.Api.Infrastructure.Data;

namespace StoreBackEnd.Api.Infrastructure.Repositories
{
    public class ProduitRepository : GenericRepository<Produit>, IProduitRepository
    {
        public ProduitRepository(ApplicationContext context) : base(context) { }

        public async Task<List<Produit>> GetProduitsSousStockMinimumAsync() =>
            await _dbSet
                .Include(p => p.Categorie)
                .Where(p => p.Stock <= p.StockMinimum)
                .ToListAsync();

        Task IProduitRepository.AddAsync(Produit produit)
        {
            return AddAsync(produit);
        }
    }
}