// Data/ChatDbContext.cs
using Microsoft.EntityFrameworkCore;
using TinyChat.Models;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

    public DbSet<ChatRoom> ChatRooms { get; set; }
}