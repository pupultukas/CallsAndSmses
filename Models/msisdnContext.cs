using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CallsAndSmses.Models
{
    public partial class msisdnContext : DbContext
    {
        public virtual DbSet<Calls> Calls { get; set; }
        public virtual DbSet<Sms> Sms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\conf.ini");
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calls>(entity =>
            {
                entity.HasKey(e => e.Date);

                entity.ToTable("calls");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DurationSeconds).HasColumnName("duration(seconds)");

                entity.Property(e => e.Msisdn).HasColumnName("msisdn");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(4)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sms>(entity =>
            {
                entity.HasKey(e => e.Date);

                entity.ToTable("sms");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Msisdn).HasColumnName("msisdn");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(3)
                    .IsUnicode(false);
            });
        }
    }
}
