using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Interfaces;
using System.Text.Json;

namespace StoreBackEnd.Api.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        public List<AuditLog> ConstruireEntreesAudit(IEnumerable<EntityEntry> entries, int? utilisateurIdCourant)
        {
            var resultats = new List<AuditLog>();

            foreach (var entry in entries)
            {
                var (anciennesValeurs, nouvellesValeurs) = ExtraireValeurs(entry);

                resultats.Add(new AuditLog
                {
                    UtilisateurID = utilisateurIdCourant,
                    Action = entry.State.ToString(), // "Added" | "Modified" | "Deleted"
                    Entite = entry.Entity.GetType().Name,
                    EntiteID = ObtenirIdEntite(entry),
                    AnciennesValeurs = anciennesValeurs,
                    NouvellesValeurs = nouvellesValeurs,
                    DateAction = DateTime.UtcNow
                });
            }

            return resultats;
        }

        private static (string? anciennes, string? nouvelles) ExtraireValeurs(EntityEntry entry)
        {
            // Added: only "after" state exists
            if (entry.State == EntityState.Added)
            {
                return (null, SerialiserProprietes(entry, useCurrentValues: true));
            }

            // Deleted: only "before" state exists 
            if (entry.State == EntityState.Deleted)
            {
                return (SerialiserProprietes(entry, useCurrentValues: false), null);
            }

            // Modified: capture both
            var avant = new Dictionary<string, object?>();
            var apres = new Dictionary<string, object?>();

            foreach (var prop in entry.Properties.Where(p => p.IsModified))
            {
                avant[prop.Metadata.Name] = prop.OriginalValue;
                apres[prop.Metadata.Name] = prop.CurrentValue;
            }

            return (
                avant.Count > 0 ? JsonSerializer.Serialize(avant) : null,
                apres.Count > 0 ? JsonSerializer.Serialize(apres) : null
            );
        }

        private static string SerialiserProprietes(EntityEntry entry, bool useCurrentValues)
        {
            var dict = entry.Properties.ToDictionary(
                p => p.Metadata.Name,
                p => useCurrentValues ? p.CurrentValue : p.OriginalValue
            );
            return JsonSerializer.Serialize(dict);
        }

        private static string? ObtenirIdEntite(EntityEntry entry)
        {
            var keyProperty = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            var value = entry.State == EntityState.Deleted ? keyProperty?.OriginalValue : keyProperty?.CurrentValue;
            return value?.ToString();
        }
    }
}