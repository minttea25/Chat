using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.DB
{
    public class AppDbContext : DbContext
    {
        // for ORM (pre-compiled string)
        const string ConnString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChatDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        const string ConnString2 = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChatDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<UserDb> Users { get; set; }
        public DbSet<ChatRoomDb> ChatRooms { get; set; }
        public DbSet<ChatDb> Chats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(Config.Configs.DBConnectionString ?? ConnString);
            optionsBuilder.UseSqlServer(ConnString2);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDb>()
                .HasIndex(a => a.AccountLoginId)
                .IsUnique();

            modelBuilder.Entity<UserDb>()
                .HasIndex(u => u.UserName)
                .IsUnique();
        }
    }
}
