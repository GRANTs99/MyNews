using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNews.Models;

namespace MyNews.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublicationItem> PublicationItems { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}