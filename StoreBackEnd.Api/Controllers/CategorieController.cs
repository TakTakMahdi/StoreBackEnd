using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBackEnd.Api.Application.Dto.Categorie;
using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Application.Services;

namespace StoreBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorieController : ControllerBase
    {
        private readonly ICategorieService _categorieService;

        public CategorieController(ICategorieService categorieService)
        {
            _categorieService = categorieService;
        }

        /// <summary>Liste les catégories avec pagination.</summary>
        /// <param name="pagination">Options de pagination.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet] // public: needed to browse products by category without logging in
        public async Task<ActionResult<PagedResultDTO<CategorieDTO>>> GetPaged(
            [FromQuery] PaginationParamsDTO pagination)
        {
            var resultat = await _categorieService.GetPagedAsync(pagination);
            return Ok(resultat);
        }

        /// <summary>Retourne une catégorie par son identifiant.</summary>
        /// <param name="categorie_id">Identifiant de la catégorie.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{categorie_id:int}")]
        public async Task<ActionResult<CategorieDTO>> GetById(int categorie_id)
        {
            var categorie = await _categorieService.GetByIdAsync(categorie_id);
            return Ok(categorie);
        }

        /// <summary>Crée une catégorie.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="dto">Données de la nouvelle catégorie.</param>
        /// <response code="201">Ressource créée.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Administrateur")]
        [HttpPost]
        public async Task<ActionResult<CategorieDTO>> Create([FromBody] CategorieCreateDTO dto)
        {
            var categorie = await _categorieService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { categorie_id = categorie.CategorieID }, categorie);
        }

        /// <summary>Modifie une catégorie.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="categorie_id">Identifiant de la catégorie à modifier.</param>
        /// <param name="dto">Nouvelles données de la catégorie.</param>
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
        [HttpPut("{categorie_id:int}")]
        public async Task<ActionResult<CategorieDTO>> Update(int categorie_id, [FromBody] CategorieCreateDTO dto)
        {
            var categorie = await _categorieService.UpdateAsync(categorie_id, dto);
            return Ok(categorie);
        }

        /// <summary>Supprime une catégorie.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="categorie_id">Identifiant de la catégorie à supprimer.</param>
        /// <response code="204">Opération réussie sans corps de réponse.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Administrateur")]
        [HttpDelete("{categorie_id:int}")]
        public async Task<IActionResult> Delete(int categorie_id)
        {
            await _categorieService.DeleteAsync(categorie_id);
            return NoContent();
        }
    }
}