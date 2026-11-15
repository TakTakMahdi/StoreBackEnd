using StoreBackEnd.Api.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace StoreBackEnd.Api.Application.Dto.Utilisateur
{
    public class RoleUpdateDTO
    {
        [Required]
        public RoleUtilisateur Role { get; set; }
    }
}