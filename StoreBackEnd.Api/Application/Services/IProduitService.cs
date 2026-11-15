using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Application.Dto.Produit;

namespace StoreBackEnd.Api.Application.Services
{
    public interface IProduitService
    {
        Task<PagedResultDTO<ProduitDTO>> GetPagedAsync(PaginationParamsDTO pagination, ProduitFiltreDTO filtre);
        Task<ProduitDTO> GetByIdAsync(int id);
        Task<ProduitDTO> CreateAsync(ProduitCreateDTO dto);
        Task<ProduitDTO> UpdateAsync(int id, ProduitUpdateDTO dto);
        Task DeleteAsync(int id);
        Task<List<ProduitDTO>> GetProduitsSousStockMinimumAsync();
    }
}