using Microsoft.EntityFrameworkCore;

namespace AccountServer.DB
{
    public class AppDbContext : DbContext
    {
        public DbSet<AccountDb>? Accounts { get; set; } = null;
        public DbSet<ChatServerIpDb>? ChatServers { get; set; } = null;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDb>()
                .HasIndex(a => a.AccountName)
                .IsUnique();

            modelBuilder.Entity<ChatServerIpDb>()
                .HasIndex(c => c.ChatServerIp)
                .IsUnique();

            modelBuilder.Entity<ChatServerIpDb>()
                .HasIndex(c => c.ChatServerName)
                .IsUnique();
        }

    }
}
