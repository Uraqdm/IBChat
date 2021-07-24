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

        [HttpGet("{chatGuid}")]
        public async Task<IEnumerable<User>> GetUsers(Guid chatGuid)
        {
            return await _context.Chats.Where(c => c.Id == chatGuid)
                .SelectMany(c => c.Members)
                .ToListAsync();
        }

        [HttpGet("userGuid")]
        public async Task<ActionResult<User>> GetUser(Guid userGuid)
        {
            var user = await _context.Users.FindAsync(userGuid);

            if (user == null) return NotFound();

            return user;
        }

        [HttpGet("userEmail")]
        public async Task<ActionResult<User>> GetUser(string passHash, string email)
        {
            return await _context.Users.Where(u => u.Password == passHash && u.Email == email).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (user == null) return BadRequest();

            await _context.Users.AddAsync(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }

            return CreatedAtAction(nameof(GetUser), new { Guid = user.Id }, user);
        }

        [HttpDelete("{userGuid}")]
        public async Task<IActionResult> DeleteUser(Guid guid)
        {
            var user = await _context.Users.FindAsync(guid);

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

        [HttpPut("{userGuid}")]
        public async Task<IActionResult> ChangeUser(Guid guid, User user)
        {
            if (guid != user.Id) return BadRequest();

            var oldUser = await _context.Users.FindAsync(guid);

            if (oldUser == null) return NotFound();

            oldUser.Email = user.Email;
            oldUser.Name = user.Name;
            oldUser.Password = user.Password;

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
