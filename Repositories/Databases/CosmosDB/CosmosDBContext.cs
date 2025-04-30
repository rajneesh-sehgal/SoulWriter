using Microsoft.Azure.Cosmos;
using SoulWriter.Configurations;

namespace SoulWriter.Repositories.Databases.CosmosDB;

public class CosmosDBContext : ICosmosDBContext
{
    private readonly CosmosClient _client;
    private readonly string _databaseName;
    private readonly string _mainContainer;    

    public CosmosDBContext(CosmosClient client, CosmosDBSettings settings)
    {
        _client = client;
        _databaseName = settings.Database;
        _mainContainer = settings.MainContainer;
    }

    public Container GetMainContainer()
    {
        return _client.GetContainer(_databaseName, _mainContainer);
    }

    public Database GetDatabase()
    {
        return _client.GetDatabase(_databaseName);
    }
    public string GetContainerName()
    {
        return _mainContainer;
    }
}