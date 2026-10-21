using EduAgentAI.Application.DTOs;
using EduAgentAI.Application.Interfaces;
using EduAgentAI.Domain.Enums;

namespace EduAgentAI.Application.Services;

public class ChatService : IChatService
{
    private readonly IAIService _aiService;
    private readonly IConversationRepository _repository;

    public ChatService(
        IAIService aiService,
        IConversationRepository repository)
    {
        _aiService = aiService;
        _repository = repository;
    }

    public async Task<ChatResponse> SendMessageAsync(
        ChatRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.StudentId == Guid.Empty)
            throw new ArgumentException("Aluno inválido.");

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException(
                "Mensagem obrigatória.");

        var message = request.Message.Trim();

        if (message.Length > 4000)
            throw new ArgumentException(
                "Mensagem excede 4000 caracteres.");

        Guid conversationId;

        if (request.ConversationId.HasValue)
        {
            conversationId = request.ConversationId.Value;

            var allowed = await _repository
                .BelongsToStudentAsync(
                    conversationId,
                    request.StudentId);

            if (!allowed)
                throw new ArgumentException(
                    "Conversa inválida ou não pertence ao aluno.");
        }
        else
        {
            conversationId = await _repository
                .CreateAsync(request.StudentId);
        }

        var history = await _repository
            .GetMessagesAsync(conversationId);

        var answer = await _aiService
            .GenerateResponseAsync(history, message);

        await _repository.AddMessageAsync(
            conversationId,
            MessageRole.Student,
            message);

        await _repository.AddMessageAsync(
            conversationId,
            MessageRole.Assistant,
            answer);

        return new ChatResponse
        {
            ConversationId = conversationId,
            Answer = answer
        };
    }
}
