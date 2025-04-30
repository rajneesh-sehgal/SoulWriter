namespace SoulWriter.Configurations;

public class ModelSettings
{
    public required string CompletionServiceId { get; set; }
    public required string EmbeddingServiceId { get; set; }
}

public class AIModel
{
    public required string ModelName { get; set; }
    public required string ServiceId { get; set; }
    public required string ModelId { get; set; }
}

public class OpenAIModel : AIModel
{
}

public class OpenAISettings
{
    public required string ApiKey { get; set; }
    public required List<OpenAIModel> CompletionModels { get; set; }
    public required List<OpenAIModel> EmbeddingModels { get; set; }
}

public class CosmosDBSettings
{
    public required string EndpointUri { get; set; }
    public required string PrimaryKey { get; set; }
    public required string Database { get; set; }
    public required string MainContainer { get; set; }
}
