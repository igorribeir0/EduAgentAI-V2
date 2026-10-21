using EduAgentAI.Application.DTOs;

namespace EduAgentAI.Application.Interfaces;

public interface IChatService
{
    Task<ChatResponse> SendMessageAsync(ChatRequest request);
}
