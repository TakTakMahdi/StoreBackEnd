using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Commande
{
    public class CommandeRejeterDTO
    {
        [Required(ErrorMessage = "Le motif de rejet est obligatoire.")]
        [StringLength(300)]
        public string MotifRejet { get; set; }
    }
}