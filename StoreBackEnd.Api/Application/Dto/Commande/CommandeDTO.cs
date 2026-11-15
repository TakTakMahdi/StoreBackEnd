namespace StoreBackEnd.Api.Application.Dto.Commande
{
    public class CommandeDTO
    {
        public int CommandeID { get; set; }
        public int ClientID { get; set; }
        public string ClientNom { get; set; }
        public int? LivreurID { get; set; }
        public string? LivreurNom { get; set; }
        public DateTime DateCommande { get; set; }
        public string Statut { get; set; }
        public string? MotifRejet { get; set; }
        public decimal Total { get; set; }
        public List<LigneCommandeDTO> Lignes { get; set; } = new();
    }
}