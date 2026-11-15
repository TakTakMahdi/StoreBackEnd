using StoreBackEnd.Api.Application.Dto.Auth;
using StoreBackEnd.Api.Application.Dto.Utilisateur;

namespace StoreBackEnd.Api.Application.Services
{
    public interface IAuthService
    {
        Task<UtilisateurDTO> RegisterAsync(RegisterDTO dto);
        Task<TokenResponseDTO> LoginAsync(LoginDTO dto);
        Task<TokenResponseDTO> RefreshAsync(RefreshRequestDTO dto);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}