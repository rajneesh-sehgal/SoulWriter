using SoulWriter.Models;

namespace SoulWriter.Helpers
{
    public static class ContextGenerator
    {
        public static string GenerateConversationContext(List<MessageModel> messages, int maxTurns = 4)
        {
            var lastMessages = messages
                .TakeLast(maxTurns * 2) // Each turn is User + Bot
                .Select(m =>
                {
                    var prefix = m.Sender == "You" ? "User:" : "SoulWriter:";
                    return $"{prefix} {Truncate(m.Text, 400)}";
                });

            return string.Join("\n", lastMessages);
        }

        private static string Truncate(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength) + "...";
        }
    }
}
