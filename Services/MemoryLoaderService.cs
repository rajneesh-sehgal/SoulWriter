using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using SoulWriter.MemoryModels;
using SoulWriter.Models;
using SoulWriter.Repositories.Databases.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoulWriter.Services;

public class MemoryLoaderService : IMemoryLoaderService
{
    private readonly MemoryService _memoryService;
    private readonly IDatabaseRepository<UserProfile> _userProfileRepository;

    public MemoryLoaderService(MemoryService memoryService, IDatabaseRepository<UserProfile> userProfileRepository)
    {
        _memoryService = memoryService;
        _userProfileRepository = userProfileRepository;
    }

    public async Task InjectUserProfileIntoMemoryAsync(string partitionKey, string userId)
    {
        var userProfile = await _userProfileRepository.FindFirstOrDefaultAsync(partitionKey, x => x.Id == userId)
                          ?? throw new Exception($"User profile with ID {userId} not found.");

        await _memoryService.SaveAsync("UserProfile", "WhoAmI", userProfile.WhoAmI ?? string.Empty);
        await _memoryService.SaveAsync("UserProfile", "HowIThink", userProfile.HowIThink ?? string.Empty);
        await _memoryService.SaveAsync("UserProfile", "MyWritingStyle", userProfile.MyWritingStyle ?? string.Empty);

        if (userProfile.TopicInterests is not null && userProfile.TopicInterests.Any())
        {
            var topics = string.Join(" | ", userProfile.TopicInterests);
            await _memoryService.SaveAsync("UserProfile", "TopicInterests", topics);
        }
    }

    public async Task<UserProfileMemory?> LoadUserProfileFromMemoryAsync()
    {
        var whoAmI = await _memoryService.GetAsync("UserProfile", "WhoAmI");
        var howIThink = await _memoryService.GetAsync("UserProfile", "HowIThink");
        var myWritingStyle = await _memoryService.GetAsync("UserProfile", "MyWritingStyle");
        var topics = await _memoryService.GetAsync("UserProfile", "TopicInterests");

        if (string.IsNullOrWhiteSpace(whoAmI) && string.IsNullOrWhiteSpace(howIThink))
        {
            return null; // Nothing stored
        }

        return new UserProfileMemory
        {
            WhoAmI = whoAmI,
            HowIThink = howIThink,
            MyWritingStyle = myWritingStyle,
            TopicInterests = topics?.Split('|', StringSplitOptions.TrimEntries).ToList() ?? new List<string>()
        };
    }
}

public interface IMemoryLoaderService
{
    Task InjectUserProfileIntoMemoryAsync(string partitionKey, string userId);
    Task<UserProfileMemory?> LoadUserProfileFromMemoryAsync();
}
