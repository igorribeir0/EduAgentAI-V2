
using EduAgentAI.Application.Interfaces;
using EduAgentAI.Application.Services;
using EduAgentAI.Infrastructure.AI;
using EduAgentAI.Infrastructure.Data;
using EduAgentAI.Infrastructure.Identity;
using EduAgentAI.Infrastructure.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 1. CONTROLLERS E OPENAPI
// =====================================================

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// =====================================================
// 2. SERVIÇOS DE CHAT
// =====================================================

builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<
    IConversationRepository,
    ConversationRepository>();

// =====================================================
// 3. GOOGLE GEMINI
// =====================================================

builder.Services.AddHttpClient<IAIService, GeminiAIService>(
    client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    });

// =====================================================
// 4. SQL SERVER / ENTITY FRAMEWORK CORE
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// =====================================================
// 5. DATA PROTECTION
// =====================================================

// Necessário para os provedores de tokens do Identity.
builder.Services.AddDataProtection();

// =====================================================
// 6. ASP.NET CORE IDENTITY
// =====================================================

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // Configuração de senha
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        // Não permitir e-mails duplicados
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// =====================================================
// 7. CONSTRUÇÃO DA APLICAÇÃO
// =====================================================




var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key não configurada.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuer não configurado.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audience não configurada.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// =====================================================
// 8. OPENAPI
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// =====================================================
// 9. MIDDLEWARES
// =====================================================

app.UseAuthentication();
app.UseAuthorization();

// =====================================================
// 10. ENDPOINTS
// =====================================================

app.MapControllers();

// =====================================================
// 11. INICIALIZAÇÃO
// =====================================================

app.Run();
