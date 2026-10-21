
namespace EduAgentAI.Application.DTOs;

public class ChatRequest
{
    public Guid StudentId { get; set; }

    public Guid? ConversationId { get; set; }

    public string Message { get; set; } = string.Empty;
}
