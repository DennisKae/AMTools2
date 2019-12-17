﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace AMTools.Web.Data.Database
{
    public class DatabaseContext : AuditBaseDatabaseContext
    {
        public DbSet<DbAppLog> AppLog { get; set; }

        public DbSet<DbAuditLog> AuditLog { get; set; }

        public DbSet<DbSubscriber> Subscriber { get; set; }

        public DbSet<DbAvailabilityStatus> AvailabilityStatus { get; set; }

        public DbSet<DbAlert> Alert { get; set; }

        public DbSet<DbUserResponse> UserResponse { get; set; }

        public DbSet<DbSetting> Setting { get; set; }

        public DatabaseContext() : base(true)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + @"C:\install\AMTools2\AMTools2.db");
            // TODO: Auf FileKonfiguration umbauen
            // TODO: Vacuum/Verkleinerung der DB einbauen: https://stackoverflow.com/questions/31127676/vacuum-sqlite-database-with-entityframework-6-1
        }

        protected override bool ObjectIsInstanceOfForbiddenClass(object target)
        {
            // Nur diese Klassen sollen geloggt werden
            bool isInstanceOfAllowedClass = target is DbAvailabilityStatus ||
                                            target is DbAlert ||
                                            target is DbUserResponse ||
                                            target is DbSubscriber;

            return !isInstanceOfAllowedClass;
        }

        protected override void AddToAuditLog(DbAuditLog dbAuditLog) => AuditLog.Add(dbAuditLog);

        protected override bool PropertyIsForbidden(object entity, string propertyName) => propertyName == "SysStampUp" || propertyName == "Xml";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder?.Entity<DbAppLog>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Timestamp);
                entity.HasIndex(x => x.ApplicationPart);
            });

            modelBuilder?.Entity<DbAuditLog>(entity =>
            {
                entity.HasIndex(x => x.TableName);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
            });

            modelBuilder?.Entity<DbSubscriber>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Issi);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder?.Entity<DbAvailabilityStatus>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Issi);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder?.Entity<DbAlert>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.Number);
                entity.HasIndex(x => x.Timestamp);
                entity.HasIndex(x => x.SysDeleted);
                entity.HasIndex(x => x.Enabled);

                entity.Property(x => x.Enabled).HasDefaultValue(true);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder?.Entity<DbUserResponse>(entity =>
            {
                entity.HasQueryFilter(x => !x.SysDeleted);
                entity.HasIndex(x => x.AlertId);
                entity.HasIndex(x => x.Issi);

                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
                entity.Property(x => x.SysDeleted).HasDefaultValue(false);
            });

            modelBuilder?.Entity<DbSetting>(entity =>
            {
                entity.Property(x => x.SysStampIn).HasDefaultValueSql("datetime('now','localtime')");
            });
        }

    }
}
