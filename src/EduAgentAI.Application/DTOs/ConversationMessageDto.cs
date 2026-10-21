
using EduAgentAI.Domain.Enums;

namespace EduAgentAI.Application.DTOs;

public class ConversationMessageDto
{
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
