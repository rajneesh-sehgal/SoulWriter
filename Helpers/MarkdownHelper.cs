using Markdig;

namespace SoulWriter.Helpers;

public static class MarkdownHelper
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

    public static string ToHtml(string markdown)
    {
        return Markdown.ToHtml(markdown ?? "", Pipeline);
    }
}
