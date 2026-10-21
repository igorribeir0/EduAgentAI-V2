
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using EduAgentAI.Application.DTOs;
using EduAgentAI.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduAgentAI.Api.Controllers;

[ApiController]
[Authorize]
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
        // Identifica o aluno pelo JWT validado.
        var userId = User.FindFirstValue(
            JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(userId, out var studentId))
        {
            return Unauthorized(new
            {
                error = "Usuário não autenticado."
            });
        }

        // Valida somente os dados da mensagem.
        if (request is null ||
            string.IsNullOrWhiteSpace(request.Message) ||
            request.Message.Length > 4000)
        {
            return BadRequest(new
            {
                error = "Mensagem inválida (máximo de 4000 caracteres)."
            });
        }

        // Nunca confiar no StudentId enviado pelo frontend.
        request.StudentId = studentId;

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
        catch (UnauthorizedAccessException)
        {
            return NotFound(new
            {
                error = "Conversa não encontrada."
            });
        }
    }
}
