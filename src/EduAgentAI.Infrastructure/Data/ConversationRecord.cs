namespace EduAgentAI.Infrastructure.Data;

public class ConversationRecord
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsClosed { get; set; }

    public List<MessageRecord> Messages { get; set; } = new();
}