using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Produit
{
    public class ProduitCreateDTO
    {
        [Required, StringLength(100)]
        public string Nom { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit etre superieur a 0.")]
        public decimal Prix { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int StockMinimum { get; set; }

        [Required]
        public int CategorieID { get; set; }
    }
}