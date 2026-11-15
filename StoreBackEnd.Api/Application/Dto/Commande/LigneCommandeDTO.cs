namespace StoreBackEnd.Api.Application.Dto.Commande
{
    public class LigneCommandeDTO
    {
        public int ProduitID { get; set; }
        public string ProduitNom { get; set; }  
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal SousTotal => Quantite * PrixUnitaire; 
    }
}