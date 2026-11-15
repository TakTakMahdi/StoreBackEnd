using StoreBackEnd.Api.Core.Entities;

namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface IProduitRepository : IGenericRepository<Produit>
    {
        Task AddAsync(Produit produit);
        Task<List<Produit>> GetProduitsSousStockMinimumAsync();
    }
}