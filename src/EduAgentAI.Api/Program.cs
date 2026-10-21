
using EduAgentAI.Application.Interfaces;
using EduAgentAI.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Injeção de dependência
builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

// OpenAPI em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Habilitar HTTPS quando configurarmos
// o certificado e a hospedagem.

// Autorização
app.UseAuthorization();

// Mapear endpoints
app.MapControllers();

app.Run();
