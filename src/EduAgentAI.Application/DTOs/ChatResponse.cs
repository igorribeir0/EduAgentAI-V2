namespace EduAgentAI.Application.DTOs;

public class ChatResponse
{
    public Guid ConversationId { get; set; }
    public string Answer { get; set; } = string.Empty;
}
