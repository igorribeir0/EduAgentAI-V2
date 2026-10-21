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
        if (request.StudentId == Guid.Empty ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                error = "StudentId e Message são obrigatórios."
            });
        }

        var response = await _chatService.SendMessageAsync(request);

        return Ok(response);
    }
}