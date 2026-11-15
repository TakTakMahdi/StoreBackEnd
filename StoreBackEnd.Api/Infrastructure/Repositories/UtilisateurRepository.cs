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

        public async Task<Utilisateur?> GetByIdWithRefreshTokenAsync(int utilisateurId, string refreshToken) =>
            await _dbSet
                .Include(u => u.RefreshTokens.Where(rt => rt.Token == refreshToken))
                .FirstOrDefaultAsync(u => u.UtilisateurID == utilisateurId);

        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
        {
            var candidats = await _context.RefreshTokens
                .Where(rt => rt.Token == refreshToken)
                .ToListAsync();

            // SQL Server may compare strings without regard to case or trailing spaces.
            return candidats.FirstOrDefault(rt => string.Equals(rt.Token, refreshToken, StringComparison.Ordinal));
        }
    }
}