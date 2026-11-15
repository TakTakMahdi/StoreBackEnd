using AutoMapper;
using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Application.Dto.Produit;
using StoreBackEnd.Api.Application.Dto.Utilisateur;
using StoreBackEnd.Api.Core.Enums;
using StoreBackEnd.Api.Core.Exceptions;
using StoreBackEnd.Api.Core.Interfaces;

namespace StoreBackEnd.Api.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResultDTO<UtilisateurDTO>> GetUtilisateursAsync(PaginationParamsDTO pagination)
        {
            var (items, total) = await _unitOfWork.Utilisateurs.GetPagedAsync(
                pagination.Page, pagination.PageSize);

            return new PagedResultDTO<UtilisateurDTO>
            {
                Items = _mapper.Map<List<UtilisateurDTO>>(items),
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = total
            };
        }

        public async Task<UtilisateurDTO> ChangerRoleAsync(int utilisateurId, RoleUpdateDTO dto)
        {
            var utilisateur = await _unitOfWork.Utilisateurs.GetByIdAsync(utilisateurId)
                ?? throw new NotFoundException("Utilisateur", utilisateurId);

            utilisateur.Role = dto.Role;
            await _unitOfWork.Utilisateurs.UpdateAsync(utilisateur);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UtilisateurDTO>(utilisateur);
        }

        public async Task<DashboardDTO> GetDashboardAsync()
        {
            var utilisateurs = await _unitOfWork.Utilisateurs.GetAllAsync();
            var produits = await _unitOfWork.Produits.GetAllAsync();
            var commandes = await _unitOfWork.Commandes.GetAllAsync();
            var produitsStockFaible = await _unitOfWork.Produits.GetProduitsSousStockMinimumAsync();

            return new DashboardDTO
            {
                TotalUtilisateurs = utilisateurs.Count,
                TotalProduits = produits.Count,
                CommandesEnAttente = commandes.Count(c => c.Statut == StatutCommande.EnAttente),
                CommandesLivrees = commandes.Count(c => c.Statut == StatutCommande.Livree),
                CommandesRejetees = commandes.Count(c => c.Statut == StatutCommande.Rejetee),
                ChiffreAffairesTotal = commandes
                    .Where(c => c.Statut == StatutCommande.Livree)
                    .Sum(c => c.Total),
                ProduitsStockFaible = _mapper.Map<List<ProduitDTO>>(produitsStockFaible)
            };
        }
    }
}