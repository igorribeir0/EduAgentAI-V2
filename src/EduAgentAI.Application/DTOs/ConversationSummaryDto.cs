namespace EduAgentAI.Application.DTOs;

public class ConversationSummaryDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
}
