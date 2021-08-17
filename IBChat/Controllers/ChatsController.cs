﻿using IBChat.Domain.Context;
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

        [HttpGet("{chatId}")]
        public async Task<ActionResult<Chat>> GetChat(Guid chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);

            if (chat == null)
                return NotFound();

            return chat;
        }

        [HttpGet("UserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetUserChats(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            return await _context.ChatMembers.Where(member => member.User.Id == userId)
                .Select(member => member.Chat)
                .ToListAsync();
        }

        [HttpPost("{ownerId}")]
        public async Task<IActionResult> CreateChat(Guid ownerId, Chat chat)
        {
            if (chat == null)
                return BadRequest();

            var owner = await _context.Users.FindAsync(ownerId);

            if (owner == null)
                return NotFound();

            _context.Chats.Add(chat);
            _context.ChatMembers.Add(new ChatMember { Chat = await _context.Chats.FindAsync(chat.Id), User = owner }); 

            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetChat), new { chatId = chat.Id}, chat);
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

            foreach (var member in _context.ChatMembers.Where(m => m.Chat.Id == chatId))
            {
                _context.ChatMembers.Remove(member);
            }

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
