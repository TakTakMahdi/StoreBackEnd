using Microsoft.EntityFrameworkCore;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Interfaces;
using StoreBackEnd.Api.Infrastructure.Data;

namespace StoreBackEnd.Api.Infrastructure.Repositories
{
    public class UtilisateurRepository : GenericRepository<Utilisateur>, IUtilisateurRepository
    {
        public UtilisateurRepository(ApplicationContext context) : base(context) { }

        public async Task<Utilisateur?> GetByEmailAsync(string email) =>
            await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}