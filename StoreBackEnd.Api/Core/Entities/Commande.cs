using StoreBackEnd.Api.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBackEnd.Api.Core.Entities
{
    public class Commande
    {
        [Key]
        public int CommandeID { get; set; }

        [ForeignKey("ClientID")]
        public int ClientID { get; set; }
        public virtual Utilisateur Client { get; set; }

        // nullable cuz a command cannot have directly a deliverer
        [ForeignKey("LivreurID")]
        public int? LivreurID { get; set; }
        public virtual Utilisateur? Livreur { get; set; }

        public DateTime DateCommande { get; set; } = DateTime.UtcNow;

        [Required]
        public StatutCommande Statut { get; set; } = StatutCommande.EnAttente;

        // if statut==rejetee 
        [StringLength(300)]
        public string? MotifRejet { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public virtual ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
    }
}