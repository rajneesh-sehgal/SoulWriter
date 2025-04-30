using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SoulWriter.Services.Agents;

public class TalkingPointsAgentService
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;

    public TalkingPointsAgentService(Kernel kernel)
    {
        _kernel = kernel;

        var yamlPromptTemplate = File.ReadAllText("./Agents/TalkingPointsAgent/TalkingPointsAgent.yaml");
        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(yamlPromptTemplate);
        var factory = new KernelPromptTemplateFactory();
        _agent = new ChatCompletionAgent(templateConfig, factory)
        {
            Kernel = _kernel            
        };
    }

    public async Task<string> GenerateTalkingPointsAsync(
        string chatCompletionServiceId,
        string userNotes,
        string whoAmI,
        string howIThink,
        string myWritingStyle,
        string topicInterests)
    {
        var args = new KernelArguments(new OpenAIPromptExecutionSettings()
        {
            ServiceId = chatCompletionServiceId
        })
        {
            { "userNotes", userNotes },
            { "whoAmI", whoAmI },
            { "howIThink", howIThink },
            { "myWritingStyle", myWritingStyle },
            { "topicInterests", topicInterests }
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
