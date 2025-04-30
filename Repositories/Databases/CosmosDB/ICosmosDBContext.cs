using Microsoft.Azure.Cosmos;

namespace SoulWriter.Repositories.Databases.CosmosDB;

public interface ICosmosDBContext
{
    Container GetMainContainer();
    Database GetDatabase();
    string GetContainerName();
}
