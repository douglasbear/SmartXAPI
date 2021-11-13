using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SmartxAPI.Models;
using SmartxAPI.Dtos.SP;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Data
{
    public partial class SmartxContext : DbContext
    {
        public SmartxContext()
        {
        }
        //private readonly IConfiguration _config;
        public SmartxContext(DbContextOptions<SmartxContext> options,IConfiguration config)
            : base(options)
        {
        }


        /*    SP Dtos           */

         public virtual DbSet<SP_LOGIN_CLOUD> SP_LOGIN_CLOUD { get; set; }



        /*     END OF SP Dtos   */

        public virtual DbSet<LanLanguage> LanLanguage { get; set; }
        public virtual DbSet<SecUser> SecUser { get; set; }
        public virtual DbSet<VwUserMenus> VwUserMenus { get; set; }
        public virtual DbSet<VwLanMultilingual> VwLanMultilingual { get; set; }
        

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseSqlServer(_config.GetConnectionString("SmartxConnection"));
        //     }
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LanLanguage>(entity =>
            {
                entity.Property(e => e.NLanguageId).ValueGeneratedNever();

                entity.Property(e => e.BRightToLeft).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsCurrent).HasDefaultValueSql("((0))");

                entity.Property(e => e.XLanguage).IsUnicode(false);
            });
            modelBuilder.Entity<SecUser>(entity =>
            {
                entity.HasKey(e => new { e.NCompanyId, e.NUserId });

                entity.Property(e => e.XPassword).IsUnicode(false);

                entity.Property(e => e.XUserId).IsUnicode(false);

                entity.Property(e => e.XUserName).IsUnicode(false);

            });

            modelBuilder.Entity<VwUserMenus>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vwUserMenus");

                entity.Property(e => e.XCaption).IsUnicode(false);

                entity.Property(e => e.XCaptionAr).IsUnicode(false);

                entity.Property(e => e.XFormNameWithTag).IsUnicode(false);

                entity.Property(e => e.XMenuName).IsUnicode(false);

                entity.Property(e => e.XShortcutKey).IsUnicode(false);

                entity.Property(e => e.XUserCategory).IsUnicode(false);
            });
            modelBuilder.Entity<VwLanMultilingual>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_LanMultilingual");

                entity.Property(e => e.Arabic).IsUnicode(false);

                entity.Property(e => e.English).IsUnicode(false);

                entity.Property(e => e.XControlNo).IsUnicode(false);
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
