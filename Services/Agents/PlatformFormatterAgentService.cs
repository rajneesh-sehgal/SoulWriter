using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SoulWriter.Services.Agents;

public class PlatformFormatterAgentService
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;

    public PlatformFormatterAgentService(Kernel kernel)
    {
        _kernel = kernel;

        var yamlPromptTemplate = File.ReadAllText("./Agents/PlatformFormatterAgent/PlatformFormatterAgent.yaml");
        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(yamlPromptTemplate);
        var factory = new KernelPromptTemplateFactory();
        _agent = new ChatCompletionAgent(templateConfig, factory)
        {
            Kernel = _kernel            
        };
    }

    public async Task<string> FormatForMediumAsync(
            string chatCompletionServiceId,
            string rawArticle, 
            string writingStyle)
    {
        var args = new KernelArguments(new OpenAIPromptExecutionSettings()
        {
            ServiceId = chatCompletionServiceId
        })
        {
            { "rawArticle", rawArticle },
            { "platform", "Medium" },
            { "myWritingStyle", writingStyle }
        };

        string response = string.Empty;

        // Invoke the agent without any messages, since the agent has all that it needs via the template and arguments.
        await foreach (ChatMessageContent content in _agent.InvokeAsync([], options: new() { KernelArguments = args }))
        {
            response = response + content.Content;
        }

        return response;
    }
}
