namespace EduAgentAI.Domain.Entities;

public class Conversation
{
    private readonly List<Message> _messages = new();

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsClosed { get; private set; }

    public IReadOnlyCollection<Message> Messages =>
        _messages.AsReadOnly();

    public Conversation(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("O aluno é obrigatório.");

        Id = Guid.NewGuid();
        StudentId = studentId;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddMessage(Message message)
    {
        if (IsClosed)
            throw new InvalidOperationException(
                "A conversa está encerrada.");

        ArgumentNullException.ThrowIfNull(message);

        if (message.ConversationId != Id)
            throw new ArgumentException(
                "A mensagem pertence a outra conversa.");

        _messages.Add(message);
    }

    public void Close()
    {
        IsClosed = true;
    }
}
