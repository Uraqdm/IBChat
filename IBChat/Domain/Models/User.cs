using System;
using System.Collections.Generic;

namespace IBChat.Domain.Models
{
    public class User
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Chat> Chats { get; set; }
    }
}
