using Microsoft.AspNetCore.Mvc;
using StoreBackEnd.Api.Application.Dto.Auth;
using StoreBackEnd.Api.Application.Services;

namespace StoreBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Crée un compte client.</summary>
        /// <param name="dto">Nom, adresse e-mail et mot de passe du client.</param>
        /// <response code="201">Ressource créée.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="409">Cette adresse e-mail est déjà utilisée.</response>
        [ProducesResponseType<StoreBackEnd.Api.Application.Dto.Utilisateur.UtilisateurDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("inscription")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var utilisateur = await _authService.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, utilisateur);
        }

        /// <summary>Authentifie un utilisateur et retourne une paire de jetons.</summary>
        /// <param name="dto">Adresse e-mail et mot de passe.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Identifiants incorrects.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("connexion")]
        public async Task<ActionResult<TokenResponseDTO>> Connexion([FromBody] LoginDTO dto)
        {
            var resultat = await _authService.LoginAsync(dto);
            return Ok(resultat);
        }

        /// <summary>Renouvelle la paire de jetons.</summary>
        /// <remarks>Le jeton d'accès peut être expiré. Le jeton de rafraîchissement utilisé est révoqué. Conservez les deux nouveaux jetons.</remarks>
        /// <param name="dto">Jeton d'accès et jeton de rafraîchissement de la même session.</param>
        /// <response code="200">Opération réussie.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="401">Jeton invalide, expiré ou révoqué.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("rafraichir")]
        public async Task<ActionResult<TokenResponseDTO>> Rafraichir([FromBody] RefreshRequestDTO dto)
        {
            var resultat = await _authService.RefreshAsync(dto);
            return Ok(resultat);
        }

        /// <summary>Révoque le jeton de rafraîchissement fourni.</summary>
        /// <param name="refreshToken">Jeton de rafraîchissement envoyé sous forme de chaîne JSON.</param>
        /// <response code="204">Opération réussie sans corps de réponse.</response>
        /// <response code="400">Requête invalide ou règle métier non satisfaite.</response>
        /// <response code="404">Ressource introuvable.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("deconnexion")]
        public async Task<IActionResult> Deconnexion([FromBody] string refreshToken)
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken);
            return NoContent();
        }
    }
}