using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserOnboardingApi.Model;

namespace UserOnboardingApi.EFCore
{
    public class EF_DataContext : IdentityDbContext
    {
        public EF_DataContext(DbContextOptions<EF_DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
   
}
