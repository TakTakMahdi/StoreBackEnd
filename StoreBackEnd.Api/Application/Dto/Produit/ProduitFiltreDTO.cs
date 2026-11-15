namespace StoreBackEnd.Api.Application.Dto.Produit
{
    public class ProduitFiltreDTO
    {
        public int? CategorieID { get; set; }
        public string? Recherche { get; set; } 
        public decimal? PrixMin { get; set; }
        public decimal? PrixMax { get; set; }
    }
}