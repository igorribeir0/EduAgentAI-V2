using EduAgentAI.Domain.Enums;

namespace EduAgentAI.Infrastructure.Data;

public class MessageRecord
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ConversationRecord Conversation { get; set; } = null!;
}