using StoreBackEnd.Api.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Core.Entities
{
    public class Utilisateur
    {
        [Key]
        public int UtilisateurID { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nom { get; set; }

        [Required, StringLength(150), EmailAddress]
        public string? Email { get; set; }

        [Required]
        public required string MotDePasseHash { get; set; }

        [Required]
        public RoleUtilisateur Role { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // navigation properties(fk)
        public virtual ICollection<Commande> CommandesClient { get; set; } = new List<Commande>();
        public virtual ICollection<Commande> CommandesLivreur { get; set; } = new List<Commande>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}