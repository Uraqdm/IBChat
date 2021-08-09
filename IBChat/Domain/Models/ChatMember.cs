using System.ComponentModel.DataAnnotations;

namespace IBChat.Domain.Models
{
    public class ChatMember
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Chat Chat { get; set; }
    }
}
