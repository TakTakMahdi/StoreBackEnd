using StoreBackEnd.Api.Application.Dto.Commande;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Enums;
using StoreBackEnd.Api.Core.Exceptions;
using StoreBackEnd.Api.Core.Interfaces;

namespace StoreBackEnd.Api.Application.Services
{
    public class CommandeService : ICommandeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommandeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandeDTO> CreerAsync(int clientId, CommandeCreateDTO dto)
        {
            var commande = new Commande
            {
                ClientID = clientId,
                Statut = StatutCommande.EnAttente,
                DateCommande = DateTime.UtcNow
            };

            decimal total = 0;

            foreach (var ligneDto in dto.Lignes)
            {
                var produit = await _unitOfWork.Produits.GetByIdAsync(ligneDto.ProduitID)
                    ?? throw new NotFoundException("Produit", ligneDto.ProduitID);

                if (produit.Stock < ligneDto.Quantite)
                    throw new BusinessRuleException(
                        $"Stock insuffisant pour '{produit.Nom}' (disponible: {produit.Stock}, demande: {ligneDto.Quantite}).");

                var ligne = new LigneCommande
                {
                    ProduitID = produit.ProduitID,
                    Quantite = ligneDto.Quantite,
                    PrixUnitaire = produit.Prix
                };

                commande.LignesCommande.Add(ligne);
                total += ligne.Quantite * ligne.PrixUnitaire;

                produit.Stock -= ligneDto.Quantite;
                await _unitOfWork.Produits.UpdateAsync(produit);
            }

            commande.Total = total;

            await _unitOfWork.Commandes.AddAsync(commande);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(commande.CommandeID);
        }

        public async Task<List<CommandeDTO>> GetHistoriqueClientAsync(int clientId)
        {
            var commandes = await _unitOfWork.Commandes.GetHistoriqueClientAsync(clientId);
            var result = new List<CommandeDTO>();

            foreach (var commande in commandes)
            {
                result.Add(await MapperVersDTOAsync(commande));
            }

            return result;
        }

        public async Task<CommandeDTO> GetByIdAsync(int id)
        {
            var commande = await _unitOfWork.Commandes.GetByIdAsync(id)
                ?? throw new NotFoundException("Commande", id);

            return await MapperVersDTOAsync(commande);
        }

        public async Task<List<CommandeDTO>> GetCommandesParLivreurAsync(int livreurId, StatutCommande? statut)
        {
            var commandes = await _unitOfWork.Commandes.GetCommandesParLivreurAsync(livreurId, statut);
            var result = new List<CommandeDTO>();

            foreach (var commande in commandes)
            {
                result.Add(await MapperVersDTOAsync(commande));
            }

            return result;
        }

        public async Task<CommandeDTO> AssignerLivreurAsync(int commandeId, int livreurId)
        {
            var commande = await _unitOfWork.Commandes.GetByIdAsync(commandeId)
                ?? throw new NotFoundException("Commande", commandeId);

            if (commande.Statut != StatutCommande.EnAttente)
                throw new BusinessRuleException("Seule une commande en attente peut etre assignee a un livreur.");

            var livreur = await _unitOfWork.Utilisateurs.GetByIdAsync(livreurId)
                ?? throw new NotFoundException("Utilisateur", livreurId);

            if (livreur.Role != RoleUtilisateur.Livreur)
                throw new BusinessRuleException("Cet utilisateur n'a pas le role Livreur.");

            commande.LivreurID = livreurId;

            await _unitOfWork.Commandes.UpdateAsync(commande);
            await _unitOfWork.SaveChangesAsync();

            return await MapperVersDTOAsync(commande);
        }

        public async Task<CommandeDTO> ConfirmerLivraisonAsync(int commandeId, int livreurId)
        {
            var commande = await ValiderCommandePourLivreurAsync(commandeId, livreurId);

            commande.Statut = StatutCommande.Livree;

            await _unitOfWork.Commandes.UpdateAsync(commande);
            await _unitOfWork.SaveChangesAsync();

            return await MapperVersDTOAsync(commande);
        }

        public async Task<CommandeDTO> RejeterLivraisonAsync(int commandeId, int livreurId, CommandeRejeterDTO dto)
        {
            var commande = await ValiderCommandePourLivreurAsync(commandeId, livreurId);

            commande.Statut = StatutCommande.Rejetee;
            commande.MotifRejet = dto.MotifRejet;

            foreach (var ligne in commande.LignesCommande)
            {
                var produit = await _unitOfWork.Produits.GetByIdAsync(ligne.ProduitID);

                if (produit != null)
                {
                    produit.Stock += ligne.Quantite;
                    await _unitOfWork.Produits.UpdateAsync(produit);
                }
            }

            await _unitOfWork.Commandes.UpdateAsync(commande);
            await _unitOfWork.SaveChangesAsync();

            return await MapperVersDTOAsync(commande);
        }

        private async Task<Commande> ValiderCommandePourLivreurAsync(int commandeId, int livreurId)
        {
            var commande = await _unitOfWork.Commandes.GetByIdAsync(commandeId)
                ?? throw new NotFoundException("Commande", commandeId);

            if (commande.LivreurID != livreurId)
                throw new BusinessRuleException("Cette commande n'est pas assignee a ce livreur.");

            if (commande.Statut != StatutCommande.EnAttente)
                throw new BusinessRuleException("Cette commande a deja ete traitee (livree ou rejetee).");

            return commande;
        }

        private async Task<CommandeDTO> MapperVersDTOAsync(Commande commande)
        {
            var client = await _unitOfWork.Utilisateurs.GetByIdAsync(commande.ClientID);

            Utilisateur? livreur = null;
            if (commande.LivreurID.HasValue)
            {
                livreur = await _unitOfWork.Utilisateurs.GetByIdAsync(
                    commande.LivreurID.Value);
            }

            return new CommandeDTO
            {
                CommandeID = commande.CommandeID,
                ClientID = commande.ClientID,
                ClientNom = client?.Nom ?? string.Empty,
                LivreurID = commande.LivreurID,
                LivreurNom = livreur?.Nom,
                DateCommande = commande.DateCommande,
                Statut = commande.Statut.ToString(),
                MotifRejet = commande.MotifRejet,
                Total = commande.Total,
                Lignes = commande.LignesCommande.Select(l => new LigneCommandeDTO
                {
                    ProduitID = l.ProduitID,
                    ProduitNom = l.Produit?.Nom ?? string.Empty,
                    Quantite = l.Quantite,
                    PrixUnitaire = l.PrixUnitaire
                }).ToList()
            };
        }
    }
}
