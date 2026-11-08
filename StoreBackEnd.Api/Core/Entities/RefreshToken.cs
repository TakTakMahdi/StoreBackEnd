using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBackEnd.Api.Core.Entities
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenID { get; set; }

        [ForeignKey("UtilisateurID")]
        public int UtilisateurID { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }

        [Required]
        public required string Token { get; set; }

        [Required]
        public DateTime DateExpiration { get; set; }

        public bool EstRevoque { get; set; } = false;

        public string? TokenRemplacant { get; set; }

        public bool EstActif => !EstRevoque && DateExpiration > DateTime.UtcNow;
    }
}