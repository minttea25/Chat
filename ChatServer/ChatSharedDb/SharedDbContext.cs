using Microsoft.EntityFrameworkCore;

namespace ChatSharedDb
{

    public class SharedDbContext : DbContext
    {
        public const string DefaultConnString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChatSharedDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public static string? ConnString { get; private set; } = null;
        public DbSet<AuthTokenDb>? Tokens { get; set; }

        public SharedDbContext() { }

        public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                if (ConnString == null) optionsBuilder.UseSqlServer(DefaultConnString);
                else optionsBuilder.UseSqlServer(ConnString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthTokenDb>()
                .HasIndex(a => a.AccountDbId)
                .IsUnique();
        }

        public static void SetConnString(string connString)
        {
            ConnString = connString;
        }
    }
}
