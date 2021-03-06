using System;
using System.ComponentModel.DataAnnotations;

namespace IBChat.Domain.Models
{
    public class Message
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        [Required]
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
