using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class SignInVm
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
