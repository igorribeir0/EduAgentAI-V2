
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using EduAgentAI.Application.DTOs;
using EduAgentAI.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAgentAI.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/conversations")]
public class ConversationController : ControllerBase
{
    private readonly IConversationRepository _repository;

    public ConversationController(
        IConversationRepository repository)
    {
        _repository = repository;
    }

    // =====================================================
    // LISTAR CONVERSAS DO ALUNO AUTENTICADO
    // =====================================================

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConversationSummaryDto>>>
        GetConversations(CancellationToken cancellationToken)
    {
        var studentId = GetAuthenticatedStudentId();

        if (studentId is null)
            return Unauthorized("Usuário não autenticado.");

        var conversations = await _repository
            .GetConversationsAsync(
                studentId.Value,
                cancellationToken);

        return Ok(conversations);
    }

    // =====================================================
    // RECUPERAR MENSAGENS DE UMA CONVERSA
    // =====================================================

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<IReadOnlyList<ConversationMessageDto>>>
        GetMessages(
            Guid conversationId,
            CancellationToken cancellationToken)
    {
        var studentId = GetAuthenticatedStudentId();

        if (studentId is null)
            return Unauthorized("Usuário não autenticado.");

        // Confirma que a conversa pertence ao aluno.
        var allowed = await _repository
            .BelongsToStudentAsync(
                conversationId,
                studentId.Value,
                cancellationToken);

        if (!allowed)
            return NotFound("Conversa não encontrada.");

        var messages = await _repository
            .GetMessagesAsync(
                conversationId,
                cancellationToken);

        return Ok(messages);
    }

    // =====================================================
    // IDENTIFICAR USUÁRIO PELO JWT
    // =====================================================

    private Guid? GetAuthenticatedStudentId()
    {
        var userId = User.FindFirstValue(
            JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(userId, out var id)
            ? id
            : null;
    }
}
