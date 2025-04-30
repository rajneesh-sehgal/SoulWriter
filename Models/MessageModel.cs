namespace SoulWriter.Models;

public class MessageModel
{
    public string Sender { get; set; } = string.Empty; // "You" or "SoulWriter"
    public string Text { get; set; } = string.Empty;
    public bool IsMarkdown { get; set; } = true;
    public string? AgentName { get; set; }  // e.g., "TalkingPointsAgent"
    public string? AgentReason { get; set; } // From RouterAgent output
}
