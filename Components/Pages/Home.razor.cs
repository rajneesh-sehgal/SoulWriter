using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SoulWriter.Helpers;
using SoulWriter.Models;
using SoulWriter.Services;
using SoulWriter.Services.Agents;

namespace SoulWriter.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject]
    private IMemoryLoaderService MemoryLoaderService { get; set; } = default!;

    [Inject]
    private RouterAgentService RouterAgentService { get; set; } = default!;

    [Inject]
    private DraftWritingAgentService DraftWritingAgentService { get; set; } = default!;

    [Inject]
    private PlatformFormatterAgentService PlatformFormatterAgentService { get; set; } = default!;

    [Inject]
    private TalkingPointsStoreService TalkingPointsStoreService { get; set; } = default!;

    [Inject]
    private BrainstormingAgentService BrainstormingAgentService { get; set; } = default!;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private string _userId = DefaultUser.DefaultUserId;
    private string _partitionKey = DefaultUser.DefaultUserId;

    private const string _chatCompletionServiceId = "openai-gpt-4o-mini";

    private ElementReference textareaRef;
    private ElementReference chatContainerRef;

    private string UserNotes = string.Empty;
    private string UserInput = string.Empty;
    private bool IsLoading = false;

    private bool ShowAgentReason = false;
    private string SelectedAgentName = "";
    private string SelectedAgentReason = "";

    private List<MessageModel> Messages = new();

    private string WelcomeMessage = """
        Hi there, I am **SoulWriter**, your personal thinking partner.

        Whether you are feeling inspired or just a little curious, I am here to help you shape your thoughts into something meaningful — one step at a time.

        Here is how we can work together:

        1. **Start with a small note or thought.**  
            It can be a sentence, a question, or something you jotted down in a moment of insight.

        2. **Let us explore it together.**  
            I will help you brainstorm ideas, offer fresh angles, and ask questions that spark deeper reflection.

        3. **You collect what clicks.**  
            Copy and paste anything you like into your notes. When you are ready, I can help turn those into a full blog post draft.

        4. **Need formatting?**  
            Once your draft is ready, I can also help format it beautifully for Medium or other platforms.

        So, what thought or note would you like to begin with today?
        """;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task AutoResize(ChangeEventArgs _)
    {
        await JS.InvokeVoidAsync("soulWriterAutoResize", textareaRef);
    }
    private async Task ScrollToBottomAsync()
    {
        await InvokeAsync(StateHasChanged);
        await Task.Yield();
        await JS.InvokeVoidAsync("scrollChatToBottom", chatContainerRef);
    }

    private void ShowReason(string? name, string? reason)
    {
        SelectedAgentName = name ?? "Unknown Agent";
        SelectedAgentReason = reason ?? "No explanation available.";
        ShowAgentReason = true;
    }

    private string GetAgentLabel(string agent)
    {
        return agent switch
        {
            "BrainstormingAgent" => "💡 Brainstorm",
            "TalkingPointsStore" => "📌 Talking Point",
            "DraftWritingAgent" => "✍️ Draft",
            "PlatformFormatterAgent" => "📰 Medium Format",
            _ => "🧩 Unknown"
        };
    }

    private async Task SubmitMessage()
    {
        if (string.IsNullOrWhiteSpace(UserInput)) return;

        var input = UserInput.Trim();
        UserInput = "";
        Messages.Add(new MessageModel { Sender = "You", Text = input });

        await RespondToInput(input);
        await ScrollToBottomAsync();
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey && !IsLoading)
        {
            await SubmitMessage();
        }
    }

    private async Task RespondToInput(string input)
    {
        IsLoading = true;

        var conversationContext = ContextGenerator.GenerateConversationContext(Messages);

        await MemoryLoaderService.InjectUserProfileIntoMemoryAsync(_partitionKey, _userId);
        var profile = await MemoryLoaderService.LoadUserProfileFromMemoryAsync();

        var routingResult = await RouterAgentService.RouteToAgentAsync(_chatCompletionServiceId, input, conversationContext);
        var selectedAgent = routingResult?.Agent ?? "";
        var routingReason = routingResult?.Reason ?? "";
        string response = "🤖 Sorry, I am not sure what to do with that yet.";

        switch (selectedAgent)
        {
            case "BrainstormingAgent":                
                response = await BrainstormingAgentService.ChatAsync(
                    _chatCompletionServiceId,
                    input,
                    conversationContext
                );
                break;

            case "DraftWritingAgent":
                var points = UserNotes;
                response = await DraftWritingAgentService.GenerateDraftAsync(
                    _chatCompletionServiceId,
                    points,
                    profile?.WhoAmI ?? "",
                    profile?.HowIThink ?? "",
                    profile?.MyWritingStyle ?? ""
                );
                break;

            case "PlatformFormatterAgent":
                var lastDraft = Messages.LastOrDefault(m => m.AgentName == "DraftWritingAgent")?.Text;
                if (!string.IsNullOrWhiteSpace(lastDraft))
                {
                    response = await PlatformFormatterAgentService.FormatForMediumAsync(
                        _chatCompletionServiceId,
                        lastDraft,
                        profile?.MyWritingStyle ?? ""
                    );
                }
                else
                {
                    response = "⚠️ I could not find any previous draft to format.";
                }
                break;

            default:
                response = "🤖 Sorry, I am not sure what to do with that yet.";
                break;
        }

        Messages.Add(new MessageModel
        {
            Sender = "SoulWriter",
            Text = response,
            IsMarkdown = true,
            AgentName = selectedAgent,
            AgentReason = routingReason
        });

        await ScrollToBottomAsync();

        IsLoading = false;
    }
}
