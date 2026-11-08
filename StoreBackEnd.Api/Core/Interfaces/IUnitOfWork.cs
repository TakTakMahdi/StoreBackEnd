namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUtilisateurRepository Utilisateurs { get; }
        IGenericRepository<Entities.Categorie> Categories { get; }
        IProduitRepository Produits { get; }
        ICommandeRepository Commandes { get; }
        IGenericRepository<Entities.LigneCommande> LignesCommande { get; }

        Task<int> SaveChangesAsync();
    }
}