namespace EduAgentAI.Application.DTOs;

public class ChatRequest
{
    public Guid StudentId { get; set; }

    // Nulo: inicia nova conversa
    // Preenchido: continua conversa existente
    public Guid? ConversationId { get; set; }

    public string Message { get; set; } = string.Empty;
}
