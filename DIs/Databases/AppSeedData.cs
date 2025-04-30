using SoulWriter.Models;
using SoulWriter.Repositories.Databases.CosmosDB;

namespace SoulWriter.DIs.Databases;

public class AppSeepData
{
    public static async Task Populate(ICosmosDBContext context)
    {
        var userProfileRepository = new CosmosDBRepository<UserProfile>(context);
        var defaultUser = await userProfileRepository.FindFirstOrDefaultAsync(DefaultUser.DefaultUserId, x => x.Id == DefaultUser.DefaultUserId);
        if (defaultUser == null)
        {
            var userProfile = UserProfile.GetDefaultUser();
            await userProfileRepository.UpsertAsync(DefaultUser.DefaultUserId, userProfile);
        }
    }
}
