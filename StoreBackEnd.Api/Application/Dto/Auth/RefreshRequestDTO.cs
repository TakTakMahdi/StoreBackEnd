namespace StoreBackEnd.Api.Application.Dto.Auth
{
    public class RefreshRequestDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}