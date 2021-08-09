using IBChat.Domain.Context;
using IBChat.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IBChat.Controllers
{
    public class UsersController : ApiBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Chat/{chatId}")]
        public async Task<IEnumerable<User>> GetChatUsers(Guid chatId)
        {
            return await _context.ChatMembers.Where(member => member.Chat.Id == chatId).Select(member => member.User).ToListAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            return user;
        }

        [HttpPut("Auth/")]
        public async Task<ActionResult<User>> Authorize(Token token)
        {
            var user = await _context.Users.Where(u => u.Email == token.Email && u.Password == token.Password).FirstOrDefaultAsync();

            if (user == null) return NotFound();

            return user;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (user == null) return BadRequest();

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {

            }
            

            return NoContent();
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> ChangeUser(Guid userId, User user)
        {
            if (userId != user.Id) return BadRequest();

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }

            return CreatedAtAction(nameof(GetUser), new { Guid = user.Id }, user);
        }

        [HttpPost("AddChat/")]
        public async Task<Chat> AddChatForUser(ChatMember newMember)
        {
            _context.ChatMembers.Add(newMember);

            await _context.SaveChangesAsync();

            return await _context.Chats.FindAsync(newMember.Chat.Id);
        }
    }
}
