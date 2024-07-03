// Repositories/ChatRoomRepository.cs
using Microsoft.EntityFrameworkCore;
using TinyChat.Models;
using TinyChat.Data;

public class ChatRoomRepository : IChatRoomRepository
{
    private readonly ChatDbContext _context;

    public ChatRoomRepository(ChatDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatRoom>> GetAllRoomsAsync()
    {
        return await _context.ChatRooms.ToListAsync();
    }

    public async Task<ChatRoom> GetRoomByNameAsync(string name)
    {
        return await _context.ChatRooms.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task CreateRoomAsync(ChatRoom room)
    {
        await _context.ChatRooms.AddAsync(room);
        await _context.SaveChangesAsync();
    }
}