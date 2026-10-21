
using EduAgentAI.Application.DTOs;
using EduAgentAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduAgentAI.Api.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationController : ControllerBase
{
    private readonly IConversationRepository _repository;

    public ConversationController(IConversationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConversationSummaryDto>>>
        GetConversations([FromQuery] Guid studentId)
    {
        if (studentId == Guid.Empty)
            return BadRequest("Aluno inválido.");

        var conversations = await _repository
            .GetConversationsAsync(studentId);

        return Ok(conversations);
    }

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<IReadOnlyList<ConversationMessageDto>>>
        GetMessages(
            Guid conversationId,
            [FromQuery] Guid studentId)
    {
        if (studentId == Guid.Empty)
            return BadRequest("Aluno inválido.");

        var allowed = await _repository.BelongsToStudentAsync(
            conversationId,
            studentId);

        if (!allowed)
            return NotFound("Conversa não encontrada.");

        var messages = await _repository
            .GetMessagesAsync(conversationId);

        return Ok(messages);
    }
}
