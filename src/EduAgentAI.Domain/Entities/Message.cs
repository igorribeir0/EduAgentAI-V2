using EduAgentAI.Domain.Enums;

namespace EduAgentAI.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public MessageRole Role { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Message(
        Guid conversationId,
        MessageRole role,
        string content)
    {
        if (conversationId == Guid.Empty)
            throw new ArgumentException("A conversa é obrigatória.");

        if (!Enum.IsDefined(role))
            throw new ArgumentException("Remetente inválido.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("A mensagem não pode estar vazia.");

        Id = Guid.NewGuid();
        ConversationId = conversationId;
        Role = role;
        Content = content.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
