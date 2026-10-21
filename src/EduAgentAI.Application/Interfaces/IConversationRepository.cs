
using EduAgentAI.Application.DTOs;
using EduAgentAI.Domain.Enums;

namespace EduAgentAI.Application.Interfaces;

public interface IConversationRepository
{
    Task<Guid> CreateAsync(
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<bool> BelongsToStudentAsync(
        Guid conversationId,
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConversationMessageDto>> GetMessagesAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    Task AddMessageAsync(
        Guid conversationId,
        MessageRole role,
        string content,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConversationSummaryDto>> GetConversationsAsync(
        Guid studentId,
        CancellationToken cancellationToken = default);
}
