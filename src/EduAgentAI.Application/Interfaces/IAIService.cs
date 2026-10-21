
namespace EduAgentAI.Application.Interfaces;

public interface IAIService
{
    Task<string> GenerateResponseAsync(
        string message,
        CancellationToken cancellationToken = default);
}
