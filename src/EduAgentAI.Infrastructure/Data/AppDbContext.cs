
using EduAgentAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduAgentAI.Infrastructure.Data;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // =====================================================
    // TABELAS EXISTENTES
    // =====================================================

    public DbSet<ConversationRecord> Conversations =>
        Set<ConversationRecord>();

    public DbSet<MessageRecord> Messages =>
        Set<MessageRecord>();

    // =====================================================
    // CONFIGURAÇÕES DO BANCO
    // =====================================================

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        // Obrigatório para configurar as tabelas do Identity
        base.OnModelCreating(modelBuilder);

        // =================================================
        // CONVERSATIONS
        // =================================================

        modelBuilder.Entity<ConversationRecord>(entity =>
        {
            entity.ToTable("Conversations");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.StudentId)
                .IsRequired();

            entity.Property(c => c.CreatedAt)
                .IsRequired();

            entity.HasIndex(c => c.StudentId);

            entity.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =================================================
        // MESSAGES
        // =================================================

        modelBuilder.Entity<MessageRecord>(entity =>
        {
            entity.ToTable("Messages");

            entity.HasKey(m => m.Id);

            entity.Property(m => m.Content)
                .HasMaxLength(4000)
                .IsRequired();

            entity.Property(m => m.Role)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(m => m.CreatedAt)
                .IsRequired();

            entity.HasIndex(m => new
            {
                m.ConversationId,
                m.CreatedAt
            });
        });
    }
}
