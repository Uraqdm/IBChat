using System;
using System.Collections.Generic;

namespace IBChat.Domain.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Message> Messages { get; set; }

    }
}
