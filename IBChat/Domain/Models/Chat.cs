using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IBChat.Domain.Models
{
    public class Chat
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Message> Messages { get; set; }

    }
}
