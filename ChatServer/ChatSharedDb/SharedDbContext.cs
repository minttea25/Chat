using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSharedDb
{

    public class SharedDbContext : DbContext
    {
        public const string ConnString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChatSharedDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public DbSet<AuthTokenDb>? Tokens { get; set; }

        public SharedDbContext() { }

        public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer(ConnString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthTokenDb>()
                .HasIndex(a => a.AccountDbId)
                .IsUnique();
        }
    }
}
