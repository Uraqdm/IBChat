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

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            return user;
        }

        [HttpGet("UserChats/{userId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetUserChats(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            return await _context.ChatMembers.Where(member => member.User.Id == userId)
                .Select(member => member.Chat)
                .ToListAsync();
        }

        [HttpPut("Auth/")]
        public async Task<ActionResult<User>> Authorize(Token token)
        {
            if(token == null)
                return BadRequest();

            var user = await _context.Tokens.Where(t => t.Email == token.Email && t.Password == token.Password).Select(t => t.User).FirstOrDefaultAsync();

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
    }
}
