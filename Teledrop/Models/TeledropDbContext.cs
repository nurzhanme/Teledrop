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

            modelBuilder.Entity<TelegramAccount>().Property(x => x.Phonenumber)
                .IsRequired();
        }

        public DbSet<TelegramAccount> TelegramAccounts { get; set; }
    }
}
