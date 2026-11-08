using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface IAuditService
    {
        List<Entities.AuditLog> ConstruireEntreesAudit(IEnumerable<EntityEntry> entries, int? utilisateurIdCourant);
    }
}