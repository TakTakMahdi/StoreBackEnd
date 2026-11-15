using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Interfaces;
using StoreBackEnd.Api.Infrastructure.Data;

namespace StoreBackEnd.Api.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;

        private IUtilisateurRepository? _utilisateurs;
        private IGenericRepository<Categorie>? _categories;
        private IProduitRepository? _produits;
        private ICommandeRepository? _commandes;
        private IGenericRepository<LigneCommande>? _lignesCommande;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
        }

        public IUtilisateurRepository Utilisateurs =>
            _utilisateurs ??= new UtilisateurRepository(_context);

        public IGenericRepository<Categorie> Categories =>
            _categories ??= new GenericRepository<Categorie>(_context);

        public IProduitRepository Produits =>
            _produits ??= new ProduitRepository(_context);

        public ICommandeRepository Commandes =>
            _commandes ??= new CommandeRepository(_context);

        public IGenericRepository<LigneCommande> LignesCommande =>
            _lignesCommande ??= new GenericRepository<LigneCommande>(_context);

         public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}