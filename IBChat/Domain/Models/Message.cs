using System;

namespace IBChat.Domain.Models
{
    public class Message
    {
        public Guid Guid { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public User Sender { get; set; }
        public Chat Chat { get; set; }
    }
}
