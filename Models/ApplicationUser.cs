using Microsoft.AspNetCore.Identity;
using TinyChat.Models;

namespace TinyChat
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Avatar { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
