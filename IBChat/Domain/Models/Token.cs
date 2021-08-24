using System;
using System.ComponentModel.DataAnnotations;

namespace IBChat.Domain.Models
{
    public class Token
    {
        [Required]
        public Guid Id { get; set; }

        public User User { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}