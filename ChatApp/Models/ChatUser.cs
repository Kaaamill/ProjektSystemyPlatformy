using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChatApp.Models;

public class ChatUser
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; }
    
    [Required]
    public string Password { get; set; }

    public ICollection<Chat> Chats { get; set; } = new List<Chat>();

}
