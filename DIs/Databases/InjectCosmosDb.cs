using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos;
using System.Collections.ObjectModel;
using SoulWriter.Configurations;
using SoulWriter.Repositories.Databases.CosmosDB;

namespace SoulWriter.DIs.Databases;

public static class InjectCosmosDb
{
    public static async Task<IServiceCollection> AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("CosmosDBSettings").Get<CosmosDBSettings>();
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings), "CosmosDBSettings cannot be null");
        }

        CosmosSerializationOptions options = new()
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        };

        var cosmosClient = new CosmosClientBuilder(settings.EndpointUri, settings.PrimaryKey)
        .WithSerializerOptions(options)
        .Build();

        var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(settings.Database);
        var database = db.Database;

        services.AddSingleton(s => cosmosClient);

        var cosmosDbContext = new CosmosDBContext(cosmosClient, settings);
        services.AddSingleton<ICosmosDBContext>(cosmosDbContext);

        var containerProperties = new ContainerProperties
        {
            Id = settings.MainContainer,
            PartitionKeyPath = "/partitionKey",
            IndexingPolicy = new IndexingPolicy
            {
                VectorIndexes =
                [
                    new VectorIndexPath 
                    { 
                        Path = @"/vectors", Type = VectorIndexType.QuantizedFlat 
                    },
                ]
            },
            VectorEmbeddingPolicy = new(
                new Collection<Embedding>(
                [
                    new Embedding
                    {
                        Path = "/vectors",
                        DataType = VectorDataType.Float32,
                        DistanceFunction = DistanceFunction.Cosine,
                        Dimensions = 1536
                    }
                ]))
        };

        var throughput = ThroughputProperties.CreateManualThroughput(1000);
        await database.CreateContainerIfNotExistsAsync(containerProperties, throughput);

        await AppSeepData.Populate(cosmosDbContext);

        return services;
    }
}