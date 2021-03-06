using IBChat.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IBChat.Domain.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions options): base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
