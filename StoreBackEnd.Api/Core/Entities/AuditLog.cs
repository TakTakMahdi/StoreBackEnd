using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBackEnd.Api.Core.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditLogID { get; set; }

        // nullable:cuz (failed login attempt) have no authenticated user
        [ForeignKey("UtilisateurID")]
        public int? UtilisateurID { get; set; }
        public virtual Utilisateur? Utilisateur { get; set; }

        [Required]
        [StringLength(20)]
        public string Action { get; set; } 

        [Required]
        [StringLength(100)]
        public string Entite { get; set; } 

        public string? EntiteID { get; set; }

        public DateTime DateAction { get; set; } = DateTime.UtcNow;
        // JSON
        public string? AnciennesValeurs { get; set; } 
        public string? NouvellesValeurs { get; set; } 
    }
}