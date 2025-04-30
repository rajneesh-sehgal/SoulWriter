using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using SoulWriter.Configurations;
using SoulWriter.Services.Agents;
using SoulWriter.Services;

namespace SoulWriter.DIs.AI;

public static class InjectSemanticKernel
{
    public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration config)
    {
        var kernel = services.AddKernel();

        var openAISettings = config.GetSection("OpenAISettings").Get<OpenAISettings>()
                                    ?? throw new InvalidOperationException("OpenAISettings section is missing or invalid.");

        foreach (var model in openAISettings.CompletionModels)
        {
            kernel.Services.AddOpenAIChatCompletion(model.ModelId, openAISettings.ApiKey, serviceId: model.ServiceId);
        }

        services.AddSingleton<MemoryService>();
        services.AddSingleton<IMemoryLoaderService, MemoryLoaderService>();
        services.AddSingleton<TalkingPointsStoreService>();

        services.AddSingleton<RouterAgentService>();
        services.AddSingleton<BrainstormingAgentService>();
        services.AddSingleton<DraftWritingAgentService>();
        services.AddSingleton<PlatformFormatterAgentService>();        

        return services;
    }
}
