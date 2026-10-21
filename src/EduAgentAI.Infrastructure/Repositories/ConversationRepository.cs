
using EduAgentAI.Application.DTOs;
using EduAgentAI.Application.Interfaces;
using EduAgentAI.Domain.Enums;
using EduAgentAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduAgentAI.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _context;

    public ConversationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Aluno inválido.");

        var conversation = new ConversationRecord
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CreatedAt = DateTime.UtcNow,
            IsClosed = false
        };

        _context.Conversations.Add(conversation);

        await _context.SaveChangesAsync(cancellationToken);

        return conversation.Id;
    }

    public async Task<bool> BelongsToStudentAsync(
        Guid conversationId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .AsNoTracking()
            .AnyAsync(c =>
                c.Id == conversationId &&
                c.StudentId == studentId &&
                !c.IsClosed,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ConversationMessageDto>> GetMessagesAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.Id)
            .Select(m => new ConversationMessageDto
            {
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task AddMessageAsync(
        Guid conversationId,
        MessageRole role,
        string content,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "A mensagem não pode estar vazia.");

        var message = new MessageRecord
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            Role = role,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
