using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Enums;
using StoreBackEnd.Api.Infrastructure.Data;
using StoreBackEnd.Api.Infrastructure.Security;

namespace StoreBackEnd.Api.Infrastructure.Seed
{
    
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationContext context)
        {
            if (!context.Utilisateurs.Any())
            {
                context.Utilisateurs.AddRange(
                    new Utilisateur
                    {
                        Nom = "Admin Principal",
                        Email = "admin@boutique.tn",
                        MotDePasseHash = PasswordHasher.Hasher("Admin@123"),
                        Role = RoleUtilisateur.Administrateur
                    },
                    new Utilisateur
                    {
                        Nom = "Livreur Test",
                        Email = "livreur@boutique.tn",
                        MotDePasseHash = PasswordHasher.Hasher("Livreur@123"),
                        Role = RoleUtilisateur.Livreur
                    },
                    new Utilisateur
                    {
                        Nom = "Client Test",
                        Email = "client@boutique.tn",
                        MotDePasseHash = PasswordHasher.Hasher("Client@123"),
                        Role = RoleUtilisateur.Client
                    }
                );

                await context.SaveChangesAsync();
            }

            if (!context.Categories.Any())
            {
                var electronique = new Categorie { Nom = "Electronique" };
                var vetements = new Categorie { Nom = "Vetements" };

                context.Categories.AddRange(electronique, vetements);
                await context.SaveChangesAsync();

                context.Produits.AddRange(
                    new Produit
                    {
                        Nom = "Smartphone X",
                        Description = "Smartphone 128GB",
                        Prix = 899.99m,
                        Stock = 20,
                        StockMinimum = 5,
                        CategorieID = electronique.CategorieID
                    },
                    new Produit
                    {
                        Nom = "Casque Audio",
                        Description = "Casque sans fil",
                        Prix = 149.50m,
                        Stock = 3, 
                        StockMinimum = 5,
                        CategorieID = electronique.CategorieID
                    },
                    new Produit
                    {
                        Nom = "T-Shirt Coton",
                        Description = "T-shirt unisexe",
                        Prix = 29.90m,
                        Stock = 50,
                        StockMinimum = 10,
                        CategorieID = vetements.CategorieID
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}