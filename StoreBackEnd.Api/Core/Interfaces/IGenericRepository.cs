using System.Linq.Expressions;

namespace StoreBackEnd.Api.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<List<T>> GetAllAsync();

        Task<(List<T> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? filtre = null,
            params Expression<Func<T, object>>[] includes);

        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task<bool> DeleteAsync(int id);
    }
}