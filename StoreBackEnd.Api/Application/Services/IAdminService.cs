using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Application.Dto.Utilisateur;

namespace StoreBackEnd.Api.Application.Services
{
    public interface IAdminService
    {
        Task<PagedResultDTO<UtilisateurDTO>> GetUtilisateursAsync(PaginationParamsDTO pagination);
        Task<UtilisateurDTO> ChangerRoleAsync(int utilisateurId, RoleUpdateDTO dto);
        Task<DashboardDTO> GetDashboardAsync();
    }
}