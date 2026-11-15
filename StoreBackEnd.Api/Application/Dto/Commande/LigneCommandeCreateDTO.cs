using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Commande
{
    public class LigneCommandeCreateDTO
    {
        [Required]
        public int ProduitID { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "La quantite doit etre au moins 1.")]
        public int Quantite { get; set; }
    }
}