using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBackEnd.Api.Core.Entities
{
    public class Produit
    {
        [Key]
        public int ProduitID { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nom { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal Prix { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public int StockMinimum { get; set; }

        [ForeignKey("CategorieID")]
        public int CategorieID { get; set; }
        //fk
        public virtual Categorie Categorie { get; set; }

        public virtual ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
    }
}