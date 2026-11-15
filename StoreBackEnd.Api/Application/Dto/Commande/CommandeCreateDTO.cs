using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Commande
{
    public class CommandeCreateDTO
    {

        [Required, MinLength(1, ErrorMessage = "La commande doit contenir au moins une ligne.")]
        public List<LigneCommandeCreateDTO> Lignes { get; set; } = new();
    }
}