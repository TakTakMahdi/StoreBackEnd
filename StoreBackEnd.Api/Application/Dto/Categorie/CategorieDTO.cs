using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Categorie
{
    public class CategorieDTO
    {
        public int CategorieID { get; set; }

        [Required]
        [StringLength(50)]
        public string Nom { get; set; }
    }
}