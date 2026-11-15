using StoreBackEnd.Api.Application.Dto.Produit;

namespace StoreBackEnd.Api.Application.Dto.Utilisateur
{
    public class DashboardDTO
    {
        public int TotalUtilisateurs { get; set; }
        public int TotalProduits { get; set; }
        public int CommandesEnAttente { get; set; }
        public int CommandesLivrees { get; set; }
        public int CommandesRejetees { get; set; }
        public decimal ChiffreAffairesTotal { get; set; } 
        public List<ProduitDTO> ProduitsStockFaible { get; set; } = new();
    }
}