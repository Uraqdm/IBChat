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
    public class MessagesController : ApiBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{Chat}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(Chat chat)
        {
            return await _context.Messages.Where(m => m.Chat.Id == chat.Id).ToListAsync();
        }


        [HttpGet("{Guid}")]
        public async Task<ActionResult<Message>> GetMessage(Guid guid)
        {
            var message =  await _context.Messages.Where(m => m.Id == guid).FirstOrDefaultAsync();

            if (message == null) 
                return NotFound();

            return message;
        }

        [HttpPost]
        public async Task<ActionResult<Message>> AddMessage(Message message)
        {
            var msg = new Message
            {
                DateTime = message.DateTime,
                Chat = message.Chat,
                Sender = message.Sender,
                Text = message.Text
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new {Guid = msg.Id }, msg);
        }

    }
}
