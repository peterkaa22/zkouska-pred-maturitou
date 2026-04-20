using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    
    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Nastavení relace User -> Notes
        modelBuilder.Entity<Note>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Když se smaže uživatel, smažou se i jeho poznámky

        // Unikátní omezení na Username
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}
