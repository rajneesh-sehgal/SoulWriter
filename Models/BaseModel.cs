using Newtonsoft.Json;

namespace SoulWriter.Models;

public abstract class BaseModel
{
    protected BaseModel()
    {
        Id = Guid.NewGuid().ToString().ToUpper();
        CreatedAt = DateTime.UtcNow;
    }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public abstract string Type { get; }
    public string Id { get; set; }
    public required string PartitionKey { get; set; }
}