using Microsoft.EntityFrameworkCore;

namespace Chat.DB
{
    public class AppDbContext : DbContext
    {
        // for ORM (pre-compiled string)
        const string ConnString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChatDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<UserDb> Users { get; set; }
        public DbSet<ChatRoomDb> ChatRooms { get; set; }
        public DbSet<ChatDb> Chats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Config.Instance != null && Config.Instance.Loaded == true && Config.Instance.Configs.DBConnectionString != null)
            {
                optionsBuilder.UseSqlServer(Config.Instance.Configs.DBConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(ConnString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDb>()
                .HasIndex(a => a.UserDbId)
                .IsUnique();
        }
    }
}
