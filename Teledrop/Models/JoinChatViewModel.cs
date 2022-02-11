using System.ComponentModel.DataAnnotations;

namespace Teledrop.Models
{
    public class JoinChatViewModel
    {
        [Required]
        public string Chatname { get; set; }
    }
}
