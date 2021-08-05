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
    public class ChatsController : ApiBase
    {
        private readonly AppDbContext _context;

        public ChatsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Id/{chatGuid}")]
        public async Task<Chat> GetChat(Guid guid)
        {
            return await _context.Chats.Where(c => c.Id == guid).FirstOrDefaultAsync();
        }

        [HttpGet("UserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetUserChats(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            return await _context.ChatMembers.Where(member => member.User.Id == userId).Select(member => member.Chat).ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(Chat chat)
        {
            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetChat), new { Guid = chat.Id}, chat);
        }

        [HttpPut("{chatId}")]
        public async Task<IActionResult> ChangeChat(Guid chatId, Chat chat)
        {
            if (chatId != chat.Id) return BadRequest();

            var _chat = await _context.Chats.FindAsync(chatId);

            _chat.Name = chat.Name;
            _chat.Messages = chat.Messages;

            try
            {
               await  _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return NotFound();
            }

            return NoContent();
            
        }

        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);

            if (chat == null) return NotFound();

            _context.Chats.Remove(chat);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }
            

            return NoContent();
        }
    }
}
