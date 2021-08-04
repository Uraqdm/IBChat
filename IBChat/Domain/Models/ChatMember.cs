namespace IBChat.Domain.Models
{
    public class ChatMember
    {
        public long Id { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
    }
}
