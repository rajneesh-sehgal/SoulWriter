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
                        I am a full-stack developer passionate about **mindfulness**, **personal development**, **fitness**, **AI**, and **storytelling**.

                        Growing up, I overcame significant challenges, including health struggles and depression, eventually transforming myself through consistent action and marathon training.

                        I deeply value:
                        - **Consistency over intensity**
                        - **Progress over perfection**
                        - **Fundamentals over fads**

                        My mission is to help others rediscover purpose and fulfillment, blending mindful living with practical technology.
                      """,
            HowIThink = """
                            I approach life with a strong sense of **mindful curiosity**, combining **practical reasoning** with **emotional intelligence**.

                            I prefer **actionable insights** over theoretical complexity and focus on **small, consistent improvements** rather than quick fixes.

                            Emotionally, I am reflective, empathetic, and committed to building systems and habits that empower long-term growth.
                        """,
            MyWritingStyle = """
                                My writing style is:
                                - **Clear and concise**, avoiding unnecessary jargon.
                                - Focused on **short paragraphs** for easy readability.
                                - Using **simple, accessible language** that feels friendly and mindful.
                                - Structured with **logical flow** from introduction to conclusion.
                                - Avoids **em-dashes** and minimizes **contractions** (preferring "do not" instead of "don't").
                                - Optimized for platforms like Medium, LinkedIn, and Twitter threads, following best practices.
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
