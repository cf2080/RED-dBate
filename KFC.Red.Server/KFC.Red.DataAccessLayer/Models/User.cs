﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFC.Red.DataAccessLayer.Models
{
    /// <summary>
    /// Model Class for User
    /// </summary>
    public class User
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public int ID { get; set; }
        public Guid SsoId { get; set; }
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Role { get; set; }

        public List<Claim> CollectionClaims { get; set; }

        public bool IsAccountActivated { get; set; } = true;


        public bool IsUserPlaying { get; set; } = false;

        public bool IsAdmin { get; set; } = false;
    }
}