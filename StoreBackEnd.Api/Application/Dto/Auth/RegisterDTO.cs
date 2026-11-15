using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Auth
{
    public class RegisterDTO
    {
        [Required, StringLength(100)]
        public string Nom { get; set; }

        [Required, StringLength(150), EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(8)]
        public string MotDePasse { get; set; }
    }
}