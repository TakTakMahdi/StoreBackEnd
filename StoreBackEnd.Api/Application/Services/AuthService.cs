using AutoMapper;
using Microsoft.Extensions.Configuration;
using StoreBackEnd.Api.Application.Dto.Auth;
using StoreBackEnd.Api.Application.Dto.Utilisateur;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Enums;
using StoreBackEnd.Api.Core.Exceptions;
using StoreBackEnd.Api.Core.Interfaces;
using StoreBackEnd.Api.Infrastructure.Security;

namespace StoreBackEnd.Api.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;   
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<UtilisateurDTO> RegisterAsync(RegisterDTO dto)
        {
            var existant = await _unitOfWork.Utilisateurs.GetByEmailAsync(dto.Email);
            if (existant != null)
                throw new ConflictException($"L'email '{dto.Email}' est deja utilise.");

            var utilisateur = new Utilisateur
            {
                Nom = dto.Nom,
                Email = dto.Email,
                MotDePasseHash = PasswordHasher.Hasher(dto.MotDePasse),
                Role = RoleUtilisateur.Client
            };

            await _unitOfWork.Utilisateurs.AddAsync(utilisateur);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UtilisateurDTO>(utilisateur);
        }

        public async Task<TokenResponseDTO> LoginAsync(LoginDTO dto)
        {
            var utilisateur = await _unitOfWork.Utilisateurs.GetByEmailAsync(dto.Email);

        
            if (utilisateur == null || !PasswordHasher.Verifier(dto.MotDePasse, utilisateur.MotDePasseHash))
                throw new UnauthorizedException("Email ou mot de passe incorrect.");

            return await GenererPaireDeTokensAsync(utilisateur);
        }

        public async Task<TokenResponseDTO> RefreshAsync(RefreshRequestDTO dto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken)
                ?? throw new UnauthorizedException("Token d'acces invalide.");

            var utilisateurIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Token d'acces invalide.");

            var utilisateurId = int.Parse(utilisateurIdClaim.Value);
            var utilisateur = await _unitOfWork.Utilisateurs
                .GetByIdWithRefreshTokenAsync(utilisateurId, dto.RefreshToken)
                ?? throw new UnauthorizedException("Utilisateur introuvable.");

            var refreshTokenEnregistre = utilisateur.RefreshTokens
                .FirstOrDefault(rt => rt.Token == dto.RefreshToken);

            if (refreshTokenEnregistre == null || !refreshTokenEnregistre.EstActif)
                throw new UnauthorizedException("Refresh token invalide ou expire.");

            // rotation: the used token is revoked and linked to its replacement,
            // so if it's ever presented again (stolen + replayed), we can detect reuse
            refreshTokenEnregistre.EstRevoque = true;

            var nouvellePaire = await GenererPaireDeTokensAsync(utilisateur);
            refreshTokenEnregistre.TokenRemplacant = nouvellePaire.RefreshToken;

            await _unitOfWork.SaveChangesAsync();

            return nouvellePaire;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _unitOfWork.Utilisateurs.GetRefreshTokenAsync(refreshToken)
                ?? throw new NotFoundException("Jeton de rafraichissement introuvable.");

            token.EstRevoque = true;
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<TokenResponseDTO> GenererPaireDeTokensAsync(Utilisateur utilisateur)
        {
            var accessToken = _tokenService.GenererAccessToken(utilisateur);
            var refreshTokenValeur = _tokenService.GenererRefreshToken();
            var dureeJours = double.Parse(_configuration["Jwt:DureeJoursRefreshToken"]!);

            var refreshToken = new RefreshToken
            {
                UtilisateurID = utilisateur.UtilisateurID,
                Token = refreshTokenValeur,
                DateExpiration = DateTime.UtcNow.AddDays(dureeJours)
            };

            utilisateur.RefreshTokens.Add(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValeur,
                ExpirationAccessToken = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:DureeMinutesAccessToken"]!))
            };
        }
    }
}