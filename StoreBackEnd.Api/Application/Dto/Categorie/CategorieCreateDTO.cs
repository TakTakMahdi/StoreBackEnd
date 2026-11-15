using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Categorie
{
    public class CategorieCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Nom { get; set; }
    }
}