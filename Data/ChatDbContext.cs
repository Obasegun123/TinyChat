using System.Reflection;
using TinyChat.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Data/ChatDbContext.cs

namespace TinyChat.Data
{
  public class ChatDbContext : IdentityDbContext<ApplicationUser>
  {
    public ChatDbContext(DbContextOptions<ChatDbContext> options)
      : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ApplicationUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Room>(ConfigureRoom);
      modelBuilder.Entity<Message>(ConfigureMessage);
    }

    private void ConfigureRoom(EntityTypeBuilder<Room> modelBuilder)
    {
      modelBuilder.ToTable("Rooms");

      modelBuilder.Property(s => s.Name).IsRequired().HasMaxLength(100);

      modelBuilder.HasOne(s => s.Admin)
          .WithMany(u => u.Rooms)
          .IsRequired();
    }

    private void ConfigureMessage(EntityTypeBuilder<Message> modelBuilder)
    {
      modelBuilder.ToTable("Messages");

      modelBuilder.Property(s => s.Content).IsRequired().HasMaxLength(500);

      modelBuilder.HasOne(s => s.ToRoom)
          .WithMany(m => m.Messages)
          .HasForeignKey(s => s.ToRoomId)
          .OnDelete(DeleteBehavior.NoAction);
    }
  }
}
