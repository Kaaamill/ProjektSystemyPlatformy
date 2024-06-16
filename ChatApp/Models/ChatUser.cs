using System.ComponentModel.DataAnnotations;

public class ChatUser
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; }
    
    [Required]
    public string Password { get; set; }
}
