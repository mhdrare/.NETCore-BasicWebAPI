using BasicWebAPI.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BasicWebAPI.Models
{
    public class BasicDbContext : IdentityDbContext<ApplicationUser>
    {
        public BasicDbContext(DbContextOptions<BasicDbContext> options) : base(options)
        {
            
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Movie> Movies { get; set; }
    }
}