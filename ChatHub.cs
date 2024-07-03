using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.Security.Claims;

[Authorize]
public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, Room> Rooms = new ConcurrentDictionary<string, Room>();

    public async Task JoinRoom(string roomName)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userAge = int.Parse(Context.User.FindFirst(ClaimTypes.DateOfBirth)?.Value ?? "0");
        var userCountry = Context.User.FindFirst(ClaimTypes.Country)?.Value;

        if (!Rooms.TryGetValue(roomName, out var room))
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
        var newRoom = new Room { MinimumAge = minimumAge, Country = country };
        if (Rooms.TryAdd(roomName, newRoom))
        {
            await Clients.All.SendAsync("RoomCreated", roomName, minimumAge, country);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Failed to create room. It may already exist.");
        }
    }

    public async Task SendMessage(string message)
    {
        var roomName = Rooms.FirstOrDefault(r => r.Value.Users.Contains(Context.ConnectionId)).Key;
        if (roomName != null)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }
    }
}

public class Room
{
    public int MinimumAge { get; set; }
    public string Country { get; set; }
    public HashSet<string> Users { get; set; } = new HashSet<string>();
}