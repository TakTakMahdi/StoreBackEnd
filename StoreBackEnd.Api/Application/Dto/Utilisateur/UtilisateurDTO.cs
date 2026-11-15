namespace StoreBackEnd.Api.Application.Dto.Utilisateur
{
    public class UtilisateurDTO
    {
        public int UtilisateurID { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } 
        public DateTime DateCreation { get; set; }
    }
}