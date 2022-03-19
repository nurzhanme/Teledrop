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
            modelBuilder.Entity<Profile>().HasIndex(x => x.Account).IsUnique();
            modelBuilder.Entity<Profile>().Property(x => x.Account).IsRequired();

            modelBuilder.Entity<Profile>().HasIndex(x => x.EvmAddress).IsUnique();

            modelBuilder.Entity<Profile>().HasIndex(x => x.YoutubeChannelId).IsUnique();

            modelBuilder.Entity<ProfileImage>().HasIndex(x => x.Account).IsUnique();
            modelBuilder.Entity<ProfileImage>().Property(x => x.Account).IsRequired();

            modelBuilder.Entity<TelegramAccount>().HasIndex(x => x.Phonenumber).IsUnique();
            modelBuilder.Entity<TelegramAccount>().Property(x => x.Phonenumber).IsRequired();

            modelBuilder.Entity<InstaAccount>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<InstaAccount>().Property(x => x.Username).IsRequired();
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileImage> ProfileImages { get; set; }
        public DbSet<TelegramAccount> TelegramAccounts { get; set; }
        public DbSet<InstaAccount> InstaAccounts { get; set; }
        public DbSet<YoutubeAuth> YoutubeAuths { get; set; }
    }
}
