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

        [HttpGet]
        public async Task<IEnumerable<Chat>> GetChats()
        {
            return await _context.Chats.ToListAsync();
        }

        [HttpGet("{chatGuid}")]
        public async Task<Chat> GetChat(Guid guid)
        {
            return await _context.Chats.Where(c => c.Id == guid).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(Chat chat)
        {
            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChat), new { Guid = chat.Id}, chat);
        }

        [HttpPut("{chatGuid}")]
        public async Task<IActionResult> ChangeChat(Guid guid, Chat chat)
        {
            if (guid != chat.Id) return BadRequest();

            var _chat = await _context.Chats.FindAsync(guid);

            _chat.Name = chat.Name;
            _chat.Messages = chat.Messages;
            _chat.Members = chat.Members;

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

        [HttpDelete("{chatGuid}")]
        public async Task<IActionResult> DeleteChat(Guid guid)
        {
            var chat = await _context.Chats.FindAsync(guid);

            if (chat == null) return NotFound();

            _context.Chats.Remove(chat);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
