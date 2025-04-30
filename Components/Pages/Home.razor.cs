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
        var intent = routingResult?.Intent ?? "";
        var routingReason = routingResult?.Reason ?? "";
        string response = "🤖 Sorry, I am not sure what to do with that yet.";

        switch (selectedAgent)
        {
            case "BrainstormingAgent":                
                if (intent == "CollaborateOnIdea")
                {
                    response = await BrainstormingAgentService.ChatAsync(
                        _chatCompletionServiceId,
                        input,
                        userNotes: UserNotes,
                        whoAmI: profile?.WhoAmI ?? "",
                        howIThink: profile?.HowIThink ?? "",
                        myWritingStyle: profile?.MyWritingStyle ?? ""
                    );
                }
                break;

            case "DraftWritingAgent":
                if (intent == "GenerateDraft")
                {
                    var points = await TalkingPointsStoreService.GetAsPlainTextAsync();
                    response = await DraftWritingAgentService.GenerateDraftAsync(
                        _chatCompletionServiceId,
                        points,
                        profile?.WhoAmI ?? "",
                        profile?.HowIThink ?? "",
                        profile?.MyWritingStyle ?? ""
                    );
                }
                break;

            case "PlatformFormatterAgent":
                if (intent == "FormatForMedium")
                {
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
                }
                break;

            case "TalkingPointsStore":
                switch (intent)
                {
                    case "AddTalkingPoint":
                        var msgToSave = Messages.LastOrDefault(m => m.Sender == "You")?.Text;
                        if (!string.IsNullOrWhiteSpace(msgToSave))
                        {
                            await TalkingPointsStoreService.AddAsync(msgToSave);
                            response = "✅ Added to your talking points list.";
                        }
                        break;

                    case "ListTalkingPoints":
                        response = await TalkingPointsStoreService.GetAsMarkdownAsync();
                        break;

                    case "DeleteTalkingPoint":
                        var deleted = await TalkingPointsStoreService.DeleteByIndexAsync(1); // use parsed index here
                        response = deleted ? "🗑️ Deleted point 2." : "⚠️ Could not delete that point.";
                        break;

                    case "ClearTalkingPoints":
                        await TalkingPointsStoreService.ClearAsync();
                        response = "🧹 Cleared all talking points.";
                        break;

                    default:
                        response = "⚠️ I am not sure what to do with that request.";
                        break;
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
