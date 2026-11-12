using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Interfaces;

namespace StoreBackEnd.Api.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        // injected so we can stamp AuditLog rows with "who did this" on every save
        private readonly IAuditService? _auditService;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationContext(
            DbContextOptions<ApplicationContext> options,
            IAuditService? auditService = null,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LignesCommande { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Client)
                .WithMany(u => u.CommandesClient)
                .HasForeignKey(c => c.ClientID)
                .OnDelete(DeleteBehavior.Restrict); // never cascade-delete a user's order history

            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Livreur)
                .WithMany(u => u.CommandesLivreur)
                .HasForeignKey(c => c.LivreurID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Categorie)
                .WithMany(c => c.Produits)
                .HasForeignKey(p => p.CategorieID)
                .OnDelete(DeleteBehavior.Restrict);
          
            modelBuilder.Entity<LigneCommande>()
                .HasOne(l => l.Commande)
                .WithMany(c => c.LignesCommande)
                .HasForeignKey(l => l.CommandeID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LigneCommande>()
                .HasOne(l => l.Produit)
                .WithMany(p => p.LignesCommande)
                .HasForeignKey(l => l.ProduitID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.Utilisateur)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UtilisateurID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Utilisateur)
                .WithMany()
                .HasForeignKey(a => a.UtilisateurID)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_auditService != null)
            {
                var entries = ChangeTracker.Entries()
                    .Where(e => e.Entity is not AuditLog 
                                && (e.State == EntityState.Added
                                    || e.State == EntityState.Modified
                                    || e.State == EntityState.Deleted))
                    .ToList();

                if (entries.Count > 0)
                {
                    int? utilisateurId = ObtenirUtilisateurIdCourant();
                    var auditEntries = _auditService.ConstruireEntreesAudit(entries, utilisateurId);
                    AuditLogs.AddRange(auditEntries);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private int? ObtenirUtilisateurIdCourant()
        //using IHttpContextAccessor
        {
            var claim = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}