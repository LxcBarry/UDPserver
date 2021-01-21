using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;

namespace DbApi.Models
{
    public partial class weathersysContext : DbContext
    {
        public weathersysContext()
        {
        }

        public weathersysContext(DbContextOptions<weathersysContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RecordTable> RecordTable { get; set; }
        public virtual DbSet<UserTable> UserTable { get; set; }
        public virtual DbSet<WeatherTable> WeatherTable { get; set; }
        public virtual DbSet<ReferenceTable> ReferenceTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var reader = new AppSettingsReader();
                var IP = reader.GetValue("serverIP", typeof(string));
                var uid = reader.GetValue("uid", typeof(string));
                var pwd = reader.GetValue("pwd", typeof(string));
                var database = reader.GetValue("database", typeof(string));
                optionsBuilder.UseMySql($"server={IP};uid={uid};pwd={pwd};database={database}", x => x.ServerVersion("5.7.31-mysql"));


            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecordTable>(entity =>
            {
                entity.HasComment("标记被记录的地点");

                entity.HasIndex(e => e.Site)
                    .HasName("RecordTable_FK_1");

                entity.HasIndex(e => e.Username)
                    .HasName("RecordTable_FK");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Site)
                    .IsRequired()
                    .HasColumnName("site")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Updatetime)
                    .HasColumnName("updatetime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<UserTable>(entity =>
            {
                entity.HasComment("用户表");

                entity.HasIndex(e => e.Username)
                    .HasName("UserTable_UN")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasColumnType("varchar(100)")
                    .HasComment("IP address")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("'123'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Updatetime)
                    .HasColumnName("updatetime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<WeatherTable>(entity =>
            {
                entity.HasComment("天气表");

                entity.HasIndex(e => e.Site)
                    .HasName("WeatherTable_UN")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("json");

                entity.Property(e => e.Site)
                    .IsRequired()
                    .HasColumnName("site")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Updatetime)
                    .HasColumnName("updatetime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });


            modelBuilder.Entity<ReferenceTable>(entity =>
            {
                entity.HasComment("参照表");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.City)
                .IsRequired()
                .HasColumnName("city")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");

                entity.Property(e => e.Citycode)
                .IsRequired()
                .HasColumnName("citycode")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");

                entity.Property(e => e.Adcode)
                .IsRequired()
                .HasColumnName("adcode")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
