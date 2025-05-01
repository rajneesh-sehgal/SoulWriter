using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SoulWriter.Models;
using System.Text.Json;

namespace SoulWriter.Services.Agents;

public class RouterAgentService
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;

    public RouterAgentService(Kernel kernel)
    {
        _kernel = kernel;

        var yamlPromptTemplate = File.ReadAllText("./Agents/RouterAgent/RouterAgent.yaml");
        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(yamlPromptTemplate);
        var factory = new KernelPromptTemplateFactory();
        _agent = new ChatCompletionAgent(templateConfig, factory)
        {
            Kernel = _kernel            
        };
    }

    public async Task<RoutingResult?> RouteToAgentAsync(string chatCompletionServiceId, string userMessage, string conversationContext)
    {
        var args = new KernelArguments(new OpenAIPromptExecutionSettings()
        {
            ServiceId = chatCompletionServiceId
        })
        {
            { "userMessage", userMessage },
            { "conversationContext", conversationContext }
        };

        string response = string.Empty;

        // Invoke the agent without any messages, since the agent has all that it needs via the template and arguments.
        await foreach (ChatMessageContent content in _agent.InvokeAsync([], options: new() { KernelArguments = args }))
        {
            response = response + content.Content;
        }

        var json = JsonDocument.Parse(response);
        return new RoutingResult
        {
            Agent = json.RootElement.GetProperty("agent").GetString() ?? "",
            Reason = json.RootElement.GetProperty("reason").GetString() ?? ""
        };
    }
}
