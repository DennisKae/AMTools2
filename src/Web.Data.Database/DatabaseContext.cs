using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AMTools.Web.Data.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<DbAppLog> AppLog { get; set; }

        public DbSet<DbSubscriber> Subscriber { get; set; }

        public DbSet<DbAvailabilityStatus> AvailabilityStatus { get; set; }

        public DbSet<DbAlert> Alert { get; set; }

        public DbSet<DbUserResponse> UserResponse { get; set; }

        public DbSet<DbSetting> Setting { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + @"C:\install\AMTools2\AMTools2.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbAppLog>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Timestamp);
                entity.HasIndex(x => x.ApplicationPart);
            });

            modelBuilder.Entity<DbSubscriber>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Issi);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<DbAvailabilityStatus>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Issi);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<DbAlert>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Number);
                entity.HasIndex(x => x.Timestamp);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<DbUserResponse>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.AlertId);
                entity.HasIndex(x => x.Issi);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<DbSetting>(entity =>
            {
                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
            });
        }
    }
}
