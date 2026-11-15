namespace StoreBackEnd.Api.Application.Dto.Produit
{
    public class ProduitDTO
    {
        public int ProduitID { get; set; }
        public string Nom { get; set; }
        public string? Description { get; set; }
        public decimal Prix { get; set; }
        public int Stock { get; set; }
        public int StockMinimum { get; set; }
        public int CategorieID { get; set; }
        public string? CategorieNom { get; set; } 
    }
}