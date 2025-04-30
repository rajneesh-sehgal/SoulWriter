using System.Linq.Expressions;

namespace SoulWriter.Repositories.Databases.Interfaces;

public interface IDatabaseRepository<T>
{
    Task<T> GetAsync(string partitionKey, string id);
    Task<bool> ExistsAsync(string partitionKey, string id);
    Task<IEnumerable<T>> GetAllAsync(string partitionKey);
    Task<IEnumerable<T>> FindAsync(string partitionKey, Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindOrderedAsync(string partitionKey, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool ascending);
    Task<IEnumerable<T>> FindTopAsync(string partitionKey, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, int top, bool ascending = true);
    Task<T?> FindFirstOrDefaultAsync(string partitionKey, Expression<Func<T, bool>> predicate);
    Task<T?> FindFirstOrDefaultOrderedAsync(string partitionKey, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool ascending);
    Task<IEnumerable<T>> QueryAsync(string partitionKey, string query, Dictionary<string, object> parameters);
    Task<List<T>> VectorSearchAsync(string partitionKey, string query, Dictionary<string, object> parameters);
    Task<List<Dictionary<string, object>>> QueryAsDictionaryAsync(string partitionKey, string query, Dictionary<string, object> parameters);
    Task AddAsync(string partitionKey, T entity);
    Task<T> UpsertAsync(string partitionKey, T entity);
    Task BatchUpsertAsync(string partitionKey, params List<T> entities);
    Task DeleteAsync(string partitionKey, string id);
}
