
using EduAgentAI.Application.Interfaces;
using EduAgentAI.Application.Services;
using EduAgentAI.Infrastructure.AI;
using EduAgentAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EduAgentAI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers e OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Chat
builder.Services.AddScoped<IChatService, ChatService>();

// Google Gemini
builder.Services.AddHttpClient<IAIService, GeminiAIService>(
    client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    });

builder.Services.AddScoped<
    IConversationRepository,
    ConversationRepository>();

// SQL Server
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
