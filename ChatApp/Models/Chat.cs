using System.Collections.Generic;

namespace ChatApp.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public ICollection<ChatUser> Participants { get; set; } = new List<ChatUser>();
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
