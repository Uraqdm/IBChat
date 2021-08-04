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
    public class UsersController : ApiBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Chat/{chatGuid}")]
        public async Task<IEnumerable<User>> GetChatUsers(Guid chatGuid)
        {
            return await _context.ChatMembers.Where(member => member.Chat.Id == chatGuid).Select(member => member.User).ToListAsync();
        }

        [HttpGet("Id/{userGuid}")]
        public async Task<ActionResult<User>> GetUser(Guid userGuid)
        {
            var user = await _context.Users.FindAsync(userGuid);

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
        public async Task<IActionResult> ChangeUser(Guid userGuid, User user)
        {
            if (userGuid != user.Id) return BadRequest();

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
        public async Task<Chat> AddChatForUser(ChatMembers newMember)
        {
            _context.ChatMembers.Add(newMember);

            await _context.SaveChangesAsync();

            return await _context.Chats.FindAsync(newMember.Chat.Id);
        }
    }
}
