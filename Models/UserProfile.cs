namespace SoulWriter.Models;

public class UserProfile : BaseModel
{
    public override string Type => nameof(UserProfile);
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? WhoAmI { get; set; }
    public string? HowIThink { get; set; }
    public string? MyWritingStyle { get; set; }
    public List<string> TopicInterests { get; set; } = [];

    public static UserProfile GetDefaultUser()
    {
        var defaultProfile = new UserProfile
        {
            Id = DefaultUser.DefaultUserId,
            PartitionKey = DefaultUser.DefaultUserId,
            Name = "Rajneesh Sehgal",
            Email = "rajneesh.sehgal@gmail.com",
            WhoAmI = """
                        I am Rajneesh Sehgal, a lifelong learner, storyteller, runner, and software developer. 
                        I have completed 3 marathons, led mindfulness workshops, and built AI-powered apps. 
                        I am on a mission to help people in their 40s who feel lost despite being successful, just like I once did.
                      """,
            HowIThink = """
                          I think deeply and reflectively. 
                          I connect ideas across disciplines — from personal growth and technology to mindfulness and storytelling. 
                          I value honesty, curiosity, and lived experience more than surface-level trends. 
                          I often see patterns and deeper truths in everyday moments. 
                          I question assumptions and like to simplify complexity.                        
                        """,
            MyWritingStyle = """
                                My writing is clear, conversational, and personal. 
                                I use short paragraphs with simple words. 
                                I speak from the heart, without trying to impress. 
                                I avoid fluff and buzzwords. 
                                I sometimes use humor or metaphor to make a point but stay grounded and relatable. 
                                I want my readers to feel like I am talking with them, not at them.                             
                             """,
            TopicInterests = [
                                "Mindfulness",
                                "Personal Growth",
                                "Fitness and Running",
                                "AI and Technology",
                                "Storytelling",
                                "Solopreneurship",
                                "Financial Independence and Personal Fulfillment"
                             ]
        };

        return defaultProfile;
    }
}
