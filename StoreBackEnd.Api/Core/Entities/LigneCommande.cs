using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBackEnd.Api.Core.Entities
{
    public class LigneCommande
    {
        [Key]
        public int LigneCommandeID { get; set; }

        [ForeignKey("CommandeID")]
        public int CommandeID { get; set; }
        public virtual Commande Commande { get; set; }

        [ForeignKey("ProduitID")]
        public int ProduitID { get; set; }
        public virtual Produit Produit { get; set; }

        [Required]
        public int Quantite { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrixUnitaire { get; set; }
    }
}