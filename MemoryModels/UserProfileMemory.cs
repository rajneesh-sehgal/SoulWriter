namespace SoulWriter.MemoryModels;

public class UserProfileMemory
{
    public string? WhoAmI { get; set; }
    public string? HowIThink { get; set; }
    public string? MyWritingStyle { get; set; }
    public List<string> TopicInterests { get; set; } = [];
}
