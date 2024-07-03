// Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using YourNamespace.Models;
using YourNamespace.Repositories;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatRoomRepository _roomRepository;

    public ChatHub(IChatRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task JoinRoom(string roomName)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userAge = int.Parse(Context.User.FindFirst(ClaimTypes.DateOfBirth)?.Value ?? "0");
        var userCountry = Context.User.FindFirst(ClaimTypes.Country)?.Value;

        var room = await _roomRepository.GetRoomByNameAsync(roomName);

        if (room == null)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Room does not exist.");
            return;
        }

        if (userAge < room.MinimumAge)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "You do not meet the age requirement for this room.");
            return;
        }

        if (room.Country != null && room.Country != userCountry)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "This room is restricted to users from a specific country.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name} has joined the room.");
    }

    public async Task CreateRoom(string roomName, int minimumAge, string country = null)
    {
        var newRoom = new ChatRoom { Name = roomName, MinimumAge = minimumAge, Country = country };
        await _roomRepository.CreateRoomAsync(newRoom);
        await Clients.All.SendAsync("RoomCreated", roomName, minimumAge, country);
    }

    // ... other methods
}