﻿using Microsoft.AspNetCore.Identity;

namespace PustokBackTask.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
