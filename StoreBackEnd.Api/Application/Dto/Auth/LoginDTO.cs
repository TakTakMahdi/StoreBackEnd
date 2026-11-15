using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Auth
{
    public class LoginDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string MotDePasse { get; set; }
    }
}