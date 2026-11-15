using StoreBackEnd.Api.Application.Dto.Categorie;
using StoreBackEnd.Api.Application.Dto.Common;

namespace StoreBackEnd.Api.Application.Services
{
    public interface ICategorieService
    {
        Task<PagedResultDTO<CategorieDTO>> GetPagedAsync(PaginationParamsDTO pagination);
        Task<CategorieDTO> GetByIdAsync(int id);
        Task<CategorieDTO> CreateAsync(CategorieCreateDTO dto);
        Task<CategorieDTO> UpdateAsync(int id, CategorieCreateDTO dto);
        Task DeleteAsync(int id);
    }
}