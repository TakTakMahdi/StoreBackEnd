namespace StoreBackEnd.Api.Application.Dto.Auth
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationAccessToken { get; set; }
    }
}