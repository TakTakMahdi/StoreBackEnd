using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Core.Entities
{
    public class Categorie
    {
        [Key]
        public int CategorieID { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nom { get; set; }

        public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
    }
}