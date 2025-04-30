using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SoulWriter.Services.Agents;

public class DraftWritingAgentService
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;

    public DraftWritingAgentService(Kernel kernel)
    {
        _kernel = kernel;

        var yamlPromptTemplate = File.ReadAllText("./Agents/DraftWritingAgent/DraftWritingAgent.yaml");
        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(yamlPromptTemplate);
        var factory = new KernelPromptTemplateFactory();
        _agent = new ChatCompletionAgent(templateConfig, factory)
        {
            Kernel = _kernel            
        };
    }

    public async Task<string> GenerateDraftAsync(
            string chatCompletionServiceId,
            string talkingPoints,
            string whoAmI,
            string howIThink,
            string myWritingStyle)
    {
        var args = new KernelArguments(new OpenAIPromptExecutionSettings()
        {
            ServiceId = chatCompletionServiceId
        })
        {
            { "talkingPoints", talkingPoints },
            { "whoAmI", whoAmI },
            { "howIThink", howIThink },
            { "myWritingStyle", myWritingStyle }
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
