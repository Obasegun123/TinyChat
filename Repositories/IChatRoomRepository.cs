// Repositories/IChatRoomRepository.cs
using TinyChat.Models;

public interface IChatRoomRepository
{
    Task<IEnumerable<ChatRoom>> GetAllRoomsAsync();
    Task<ChatRoom> GetRoomByNameAsync(string name);
    Task CreateRoomAsync(ChatRoom room);
    // Add other methods as needed
}