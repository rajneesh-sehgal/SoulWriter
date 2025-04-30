using SoulWriter.Configurations;
using SoulWriter.DIs.AI;
using SoulWriter.DIs.Databases;
using SoulWriter.Repositories.Databases.CosmosDB;
using SoulWriter.Repositories.Databases.Interfaces;
using SoulWriter.Services;
using SoulWriter.Services.Agents;

namespace SoulWriter.DIs;

public static class InjectDependencies
{
    public static IServiceCollection AddAllDependencies(
             this IServiceCollection services, IConfiguration config)
    {
        services.Configure<OpenAISettings>(config.GetSection("OpenAISettings"));
        services.Configure<ModelSettings>(config.GetSection("ModelSettings"));        

        services.AddHttpClient();

        services.AddCosmosDb(config).Wait();
        services.AddSemanticKernel(config);

        // Add repositories
        services.AddTransient(typeof(IDatabaseRepository<>), typeof(CosmosDBRepository<>));

        // Add services

        return services;
    }
}
