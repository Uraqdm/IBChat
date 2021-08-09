﻿using System;
using System.ComponentModel.DataAnnotations;

namespace IBChat.Domain.Models
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
