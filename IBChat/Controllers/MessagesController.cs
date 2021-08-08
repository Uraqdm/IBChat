using IBChat.Domain.Context;
using IBChat.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IBChat.Controllers
{
    public class MessagesController : ApiBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Chat/{chatId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(Guid chatId)
        {
            return await _context.Messages.Where(m => m.Chat.Id == chatId).ToListAsync();
        }


        [HttpGet("{messageId}")]
        public async Task<ActionResult<Message>> GetMessage(Guid messageId)
        {
            var message = await _context.Messages.Where(m => m.Id == messageId).FirstOrDefaultAsync();

            if (message == null) 
                return NotFound();

            return message;
        }

        [HttpPost]
        public async Task<ActionResult<Message>> AddMessage(Message message)
        {
            var msg = new Message
            {
                Text = message.Text,
                DateTime = DateTime.Now,
                Chat = await _context.Chats.FindAsync(message.Chat.Id),
                Sender = await _context.Users.FindAsync(message.Sender.Id)
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new {messageId = msg.Id }, msg);
        }

    }
}
