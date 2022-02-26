using Microsoft.EntityFrameworkCore;

namespace Teledrop.Models
{
    public class TeledropDbContext : DbContext
    {
        public TeledropDbContext(DbContextOptions<TeledropDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelegramAccount>().HasIndex(x => x.Phonenumber).IsUnique();
            modelBuilder.Entity<TelegramAccount>().Property(x => x.Phonenumber).IsRequired();

            modelBuilder.Entity<InstaAccount>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<InstaAccount>().Property(x => x.Username).IsRequired();
        }

        public DbSet<TelegramAccount> TelegramAccounts { get; set; }
        public DbSet<InstaAccount> InstaAccounts { get; set; }
    }
}
