using System.Net.Http.Json;
using System.Text.Json;
using EduAgentAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EduAgentAI.Infrastructure.AI;

public class GeminiAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiAIService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException(
                "Chave Gemini não configurada.");

        _model = configuration["Gemini:Model"]
            ?? "gemini-2.5-flash";
    }

    public async Task<string> GenerateResponseAsync(
        string message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "A mensagem não pode estar vazia.");

        var payload = new
        {
            system_instruction = new
            {
                parts = new[]
                {
                    new
                    {
                        text = """
                        Você é o EduAgent AI, assistente
                        virtual de uma escola de idiomas.

                        Responda sempre em português brasileiro.

                        Ajude alunos com dúvidas sobre:
                        - Aprendizado de idiomas
                        - Métodos de estudo
                        - Organização dos estudos
                        - Informações gerais sobre cursos

                        Nunca invente horários, mensalidades
                        ou informações pessoais de alunos.

                        Se não souber uma informação específica,
                        oriente o aluno a consultar a secretaria.
                        """
                    }
                }
            },
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = message }
                    }
                }
            }
        };

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(_model)}:generateContent";

        using var request =
            new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Add("x-goog-api-key", _apiKey);
        request.Content = JsonContent.Create(payload);

        using var response = await _httpClient.SendAsync(
            request, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken);

        using var json = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken);

        var candidates =
            json.RootElement.GetProperty("candidates");

        if (candidates.GetArrayLength() == 0)
            throw new InvalidOperationException(
                "O Gemini não retornou respostas.");

        var parts = candidates[0]
            .GetProperty("content")
            .GetProperty("parts");

        var answer = string.Join(
            "\n",
            parts.EnumerateArray()
                .Where(p => p.TryGetProperty("text", out _))
                .Select(p => p.GetProperty("text").GetString()));

        if (string.IsNullOrWhiteSpace(answer))
            throw new InvalidOperationException(
                "O Gemini retornou uma resposta vazia.");

        return answer.Trim();
    }
}
