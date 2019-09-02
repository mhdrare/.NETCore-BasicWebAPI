using Microsoft.AspNetCore.Identity;

namespace BasicWebAPI.Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
    }
}