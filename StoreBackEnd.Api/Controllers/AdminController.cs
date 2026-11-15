using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Application.Dto.Utilisateur;
using StoreBackEnd.Api.Application.Services;

namespace StoreBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrateur")] 
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>Liste les utilisateurs avec pagination.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="pagination">Options de pagination.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("utilisateurs")]
        public async Task<ActionResult<PagedResultDTO<UtilisateurDTO>>> GetUtilisateurs(
            [FromQuery] PaginationParamsDTO pagination)
        {
            var resultat = await _adminService.GetUtilisateursAsync(pagination);
            return Ok(resultat);
        }

        /// <summary>Modifie le rôle d'un utilisateur.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="utilisateur_id">Identifiant de l'utilisateur à modifier.</param>
        /// <param name="dto">Nouveau rôle de l'utilisateur.</param>
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
        [HttpPut("utilisateurs/{utilisateur_id:int}/role")]
        public async Task<ActionResult<UtilisateurDTO>> ChangerRole(int utilisateur_id, [FromBody] RoleUpdateDTO dto)
        {
            var utilisateur = await _adminService.ChangerRoleAsync(utilisateur_id, dto);
            return Ok(utilisateur);
        }

        /// <summary>Retourne les indicateurs du tableau de bord.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <response code="200">Opération réussie.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDTO>> GetDashboard()
        {
            var dashboard = await _adminService.GetDashboardAsync();
            return Ok(dashboard);
        }
    }
}