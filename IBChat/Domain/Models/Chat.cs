using System;
using System.Collections.Generic;

namespace IBChat.Domain.Models
{
    public class Chat
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public List<User> Members { get; set; }
        public List<Message> Messages { get; set; }

    }
}
