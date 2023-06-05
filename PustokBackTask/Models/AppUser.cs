using Microsoft.AspNetCore.Identity;

namespace PustokBackTask.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }

        public string Phone {get; set;}

        public string Adress {get; set;}
        public string ConnectionId { get; set; }
        public DateTime LastOnlineAt { get; set; }

    }
}
