using EduAgentAI.Application.DTOs;

namespace EduAgentAI.Application.Interfaces;

public interface IAIService
{
    Task<string> GenerateResponseAsync(
        IReadOnlyList<ConversationMessageDto> history,
        string message,
        CancellationToken cancellationToken = default);
}