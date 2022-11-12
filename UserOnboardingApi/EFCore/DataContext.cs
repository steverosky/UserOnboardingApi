using Microsoft.EntityFrameworkCore;

namespace UserOnboardingApi.EFCore
{
    public class EF_DataContext : DbContext
    {
        public EF_DataContext(DbContextOptions<EF_DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
   
}
