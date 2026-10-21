using EduAgentAI.Application.DTOs;
using EduAgentAI.Application.Interfaces;

namespace EduAgentAI.Application.Services;

public class ChatService : IChatService
{
    public Task<ChatResponse> SendMessageAsync(ChatRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.StudentId == Guid.Empty)
            throw new ArgumentException("Aluno inválido.");

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("A mensagem é obrigatória.");

        var response = new ChatResponse
        {
            ConversationId = Guid.NewGuid(),
            Answer = "Olá! Sou o EduAgent AI. Em breve poderei consultar suas aulas e horários."
        };

        return Task.FromResult(response);
    }
}
