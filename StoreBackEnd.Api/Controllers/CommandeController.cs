using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBackEnd.Api.Application.Dto.Commande;
using StoreBackEnd.Api.Application.Services;
using StoreBackEnd.Api.Core.Enums;
using System.Security.Claims;

namespace StoreBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class CommandeController : ControllerBase
    {
        private readonly ICommandeService _commandeService;

        public CommandeController(ICommandeService commandeService)
        {
            _commandeService = commandeService;
        }

        private int UtilisateurIdCourant =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Crée une commande pour le client connecté.</summary>
        /// <remarks>Rôle requis : Client.</remarks>
        /// <param name="dto">Lignes de commande avec les produits et les quantités.</param>
        /// <response code="201">Ressource créée.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<ActionResult<CommandeDTO>> Creer([FromBody] CommandeCreateDTO dto)
        {
            var commande = await _commandeService.CreerAsync(UtilisateurIdCourant, dto);
            return CreatedAtAction(nameof(GetById), new { command_id = commande.CommandeID }, commande);
        }

        /// <summary>Liste les commandes du client connecté.</summary>
        /// <remarks>Rôle requis : Client.</remarks>
        /// <response code="200">Opération réussie.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Client")]
        [HttpGet("mes-commandes")]
        public async Task<ActionResult<List<CommandeDTO>>> GetMesCommandes()
        {
            var commandes = await _commandeService.GetHistoriqueClientAsync(UtilisateurIdCourant);
            return Ok(commandes);
        }

        /// <summary>Retourne une commande par son identifiant.</summary>
        /// <remarks>Authentification requise.</remarks>
        /// <param name="command_id">Identifiant de la commande.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{command_id:int}")]
        public async Task<ActionResult<CommandeDTO>> GetById(int command_id)
        {
            var commande = await _commandeService.GetByIdAsync(command_id);
            return Ok(commande);
        }

        /// <summary>Assigne une commande en attente à un livreur.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="command_id">Identifiant de la commande à assigner.</param>
        /// <param name="livreur_id">Identifiant de l'utilisateur ayant le rôle Livreur.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Administrateur")]
        [HttpPut("{command_id:int}/assigner-livreur/{livreur_id:int}")]
        public async Task<ActionResult<CommandeDTO>> AssignerLivreur(int command_id, int livreur_id)
        {
            var commande = await _commandeService.AssignerLivreurAsync(command_id, livreur_id);
            return Ok(commande);
        }

        /// <summary>Liste les commandes attribuées au livreur connecté.</summary>
        /// <remarks>Rôle requis : Livreur.</remarks>
        /// <param name="statut">Statut facultatif pour filtrer les commandes.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Livreur")]
        [HttpGet("mes-livraisons")]
        public async Task<ActionResult<List<CommandeDTO>>> GetMesLivraisons([FromQuery] StatutCommande? statut)
        {
            var commandes = await _commandeService.GetCommandesParLivreurAsync(UtilisateurIdCourant, statut);
            return Ok(commandes);
        }

        /// <summary>Marque une commande assignée comme livrée.</summary>
        /// <remarks>Rôle requis : Livreur. La commande doit être assignée au livreur connecté.</remarks>
        /// <param name="command_id">Identifiant de la commande livrée.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Livreur")]
        [HttpPut("{command_id:int}/confirmer-livraison")]
        public async Task<ActionResult<CommandeDTO>> ConfirmerLivraison(int command_id)
        {
            var commande = await _commandeService.ConfirmerLivraisonAsync(command_id, UtilisateurIdCourant);
            return Ok(commande);
        }

        /// <summary>Enregistre le rejet d'une livraison.</summary>
        /// <remarks>Rôle requis : Livreur. La commande doit être assignée au livreur connecté.</remarks>
        /// <param name="command_id">Identifiant de la commande concernée.</param>
        /// <param name="dto">Motif du rejet.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Livreur")]
        [HttpPut("{command_id:int}/rejeter-livraison")]
        public async Task<ActionResult<CommandeDTO>> RejeterLivraison(int command_id, [FromBody] CommandeRejeterDTO dto)
        {
            var commande = await _commandeService.RejeterLivraisonAsync(command_id, UtilisateurIdCourant, dto);
            return Ok(commande);
        }
    }
}