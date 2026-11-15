using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Application.Dto.Produit;
using StoreBackEnd.Api.Application.Services;

namespace StoreBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProduitController : ControllerBase
    {
        private readonly IProduitService _produitService;

        public ProduitController(IProduitService produitService)
        {
            _produitService = produitService;
        }

        // public: a non-logged-in visitor can browse the store (per the cahier des charges)
        /// <summary>Liste les produits avec pagination et filtres.</summary>
        /// <param name="pagination">Options de pagination.</param>
        /// <param name="filtre">Filtres facultatifs par catégorie, texte et fourchette de prix.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<ProduitDTO>>> GetPaged(
            [FromQuery] PaginationParamsDTO pagination,
            [FromQuery] ProduitFiltreDTO filtre)
        {
            var resultat = await _produitService.GetPagedAsync(pagination, filtre);
            return Ok(resultat);
        }

        /// <summary>Retourne un produit par son identifiant.</summary>
        /// <param name="produit_id">Identifiant du produit.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{produit_id:int}")]
        public async Task<ActionResult<ProduitDTO>> GetById(int produit_id)
        {
            var produit = await _produitService.GetByIdAsync(produit_id);
            return Ok(produit);
        }

        /// <summary>Liste les produits dont le stock est inférieur ou égal au seuil minimum.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <response code="200">Opération réussie.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Administrateur")]
        [HttpGet("stock-faible")]
        public async Task<ActionResult<List<ProduitDTO>>> GetStockFaible()
        {
            var produits = await _produitService.GetProduitsSousStockMinimumAsync();
            return Ok(produits);
        }

        /// <summary>Crée un produit.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="dto">Données du produit et identifiant de sa catégorie.</param>
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
        [Authorize(Roles = "Administrateur")]
        [HttpPost]
        public async Task<ActionResult<ProduitDTO>> Create([FromBody] ProduitCreateDTO dto)
        {
            var produit = await _produitService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { produit_id = produit.ProduitID }, produit);
        }

        /// <summary>Modifie un produit.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="produit_id">Identifiant du produit à modifier.</param>
        /// <param name="dto">Nouvelles données du produit.</param>
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
        [HttpPut("{produit_id:int}")]
        public async Task<ActionResult<ProduitDTO>> Update(int produit_id, [FromBody] ProduitUpdateDTO dto)
        {
            var produit = await _produitService.UpdateAsync(produit_id, dto);
            return Ok(produit);
        }

        /// <summary>Supprime un produit du catalogue.</summary>
        /// <remarks>Rôle requis : Administrateur.</remarks>
        /// <param name="produit_id">Identifiant du produit à supprimer.</param>
        /// <response code="204">Opération réussie sans corps de réponse.</response>
        /// <response code="401">Authentification invalide ou manquante.</response>
        /// <response code="403">Rôle requis pour cette opération.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Administrateur")]
        [HttpDelete("{produit_id:int}")]
        public async Task<IActionResult> Delete(int produit_id)
        {
            await _produitService.DeleteAsync(produit_id);
            return NoContent();
        }
    }
}