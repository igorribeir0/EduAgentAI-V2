
using EduAgentAI.Application.DTOs;
using EduAgentAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduAgentAI.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("message")]
    public async Task<ActionResult<ChatResponse>> SendMessage(
        [FromBody] ChatRequest request)
    {
        if (request is null ||
            request.StudentId == Guid.Empty ||
            string.IsNullOrWhiteSpace(request.Message) ||
            request.Message.Length > 4000)
        {
            return BadRequest(new
            {
                error = "Aluno e mensagem válidos são obrigatórios (máximo de 4000 caracteres)."
            });
        }

        try
        {
            var response = await _chatService
                .SendMessageAsync(request);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }
}
