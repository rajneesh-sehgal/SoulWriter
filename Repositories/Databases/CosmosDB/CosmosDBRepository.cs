using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using SoulWriter.Repositories.Databases.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace SoulWriter.Repositories.Databases.CosmosDB;

public class CosmosDBRepository<T> : IDatabaseRepository<T> where T : class
{
    private readonly ICosmosDBContext _context;
    public CosmosDBRepository(ICosmosDBContext context)
    {
        _context = context;
    }

    public async Task<T> GetAsync(string partitionKey, string id)
    {
        var container = _context.GetMainContainer();
        ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        return response.Resource;
    }

    public async Task<bool> ExistsAsync(string partitionKey, string id)
    {
        try
        {
            var container = _context.GetMainContainer();
            ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }    

    public async Task<IEnumerable<T>> GetAllAsync(string partitionKey)
    {
        var container = _context.GetMainContainer();
        var query = container.GetItemQueryIterator<T>(requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) });
        var results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.Resource);
        }
        return results;
    }

    public async Task<IEnumerable<T>> FindAsync(string partitionKey, Expression<Func<T, bool>> predicate)
    {
        var container = _context.GetMainContainer();
        var queryable = container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) })
            .Where(predicate)
            .ToFeedIterator();

        List<T> results = new List<T>();
        while (queryable.HasMoreResults)
        {
            results.AddRange(await queryable.ReadNextAsync());
        }

        return results;
    }

    public async Task<IEnumerable<T>> FindOrderedAsync(string partitionKey, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool ascending)
    {
        var container = _context.GetMainContainer();
        var queryable = container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) })
            .Where(predicate);
        queryable = ascending ? queryable.OrderBy(orderBy) : queryable.OrderByDescending(orderBy);
        var iterator = queryable.ToFeedIterator();
        var results = new List<T>();
        while (iterator.HasMoreResults)
        {
            var feed = await iterator.ReadNextAsync();
            results.AddRange(feed);
        }
        return results;
    }

    public async Task<IEnumerable<T>> FindTopAsync(string partitionKey, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, int top, bool ascending)
    {
        var container = _context.GetMainContainer();
        var queryable = container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) })
            .Where(predicate);

        queryable = ascending ? queryable.OrderBy(orderBy) : queryable.OrderByDescending(orderBy);

        var iterator = queryable.Take(top).ToFeedIterator();

        List<T> results = new List<T>();
        while (iterator.HasMoreResults)
        {
            results.AddRange(await iterator.ReadNextAsync());
        }

        return results;
    }

    public async Task<T?> FindFirstOrDefaultOrderedAsync(string partitionKey, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool ascending)
    {
        var container = _context.GetMainContainer();
        var queryable = container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) })
            .Where(predicate);

        queryable = ascending ? queryable.OrderBy(orderBy) : queryable.OrderByDescending(orderBy);

        var iterator = queryable.ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            var feed = await iterator.ReadNextAsync();
            return feed.FirstOrDefault();
        }

        return null;
    }

    public async Task<T?> FindFirstOrDefaultAsync(string partitionKey, Expression<Func<T, bool>> predicate)
    {
        var container = _context.GetMainContainer();
        var queryable = container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) })
            .Where(predicate)
            .ToFeedIterator();

        while (queryable.HasMoreResults)
        {
            var feed = await queryable.ReadNextAsync();
            return feed.FirstOrDefault();
        }

        return null;
    }

    public async Task<IEnumerable<T>> QueryAsync(string partitionKey, string query, Dictionary<string, object> parameters)
    {
        var container = _context.GetMainContainer();
        var queryDefinition = new QueryDefinition(query);

        foreach (var parameter in parameters)
        {
            queryDefinition.WithParameter(parameter.Key, parameter.Value);
        }

        var iterator = container.GetItemQueryIterator<T>(
            queryDefinition,
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) }
        );

        var results = new List<T>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.Resource);
        }
        return results;
    }

    public async Task<List<Dictionary<string, object>>> QueryAsDictionaryAsync(string partitionKey, string query, Dictionary<string, object> parameters)
    {
        var container = _context.GetMainContainer();
        var queryDefinition = new QueryDefinition(query);

        foreach (var parameter in parameters)
        {
            queryDefinition.WithParameter(parameter.Key, parameter.Value);
        }

        var iterator = container.GetItemQueryIterator<Dictionary<string, object>>(
            queryDefinition,
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) }
        );

        var results = new List<Dictionary<string, object>>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.Resource);
        }
        return results;
    }

    public async Task<List<T>> VectorSearchAsync(string partitionKey, string query, Dictionary<string, object> parameters)
    {
        var container = _context.GetMainContainer();
        var queryDefinition = new QueryDefinition(query);

        foreach (var parameter in parameters)
        {
            queryDefinition.WithParameter(parameter.Key, parameter.Value);
        }

        var iterator = container.GetItemQueryIterator<T>(
            queryDefinition,
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) }
        );

        var results = new List<T>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task AddAsync(string partitionKey, T entity)
    {
        var container = _context.GetMainContainer();
        await container.CreateItemAsync(entity, new PartitionKey(partitionKey));
    }

    public async Task<T> UpsertAsync(string partitionKey, T entity)
    {
        var container = _context.GetMainContainer();
        var upserted = await container.UpsertItemAsync(entity, new PartitionKey(partitionKey));
        return upserted;
    }

    public async Task DeleteAsync(string partitionKey, string id)
    {
        var container = _context.GetMainContainer();
        await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
    }
    
    public async Task BatchUpsertAsync(string partitionKey, params List<T> entities)
    {
        var pk = new PartitionKey(partitionKey);
        var container = _context.GetMainContainer();
        var transactionalBatch = container.CreateTransactionalBatch(pk);

        foreach (var entity in entities)
        {
            transactionalBatch.UpsertItem(entity);
        }

        await transactionalBatch.ExecuteAsync();
    }
}
