
using EduAgentAI.Application.Interfaces;
using EduAgentAI.Application.Services;
using EduAgentAI.Infrastructure.AI;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Documentação OpenAPI
builder.Services.AddOpenApi();

// Serviço de chat
builder.Services.AddScoped<IChatService, ChatService>();

// Integração com Google Gemini
builder.Services.AddHttpClient<IAIService, GeminiAIService>(
    client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
