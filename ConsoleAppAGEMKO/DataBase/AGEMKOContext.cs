using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ConsoleAppAGEMKO.DataBase
{
    public partial class AGEMKOContext : DbContext
    {
        public AGEMKOContext()
        {
        }

        public AGEMKOContext(DbContextOptions<AGEMKOContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MainTable> MainTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DEV-STAVROU;Database=AGEMKO;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<MainTable>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Afm)
                    .IsRequired()
                    .HasColumnName("AFM")
                    .HasMaxLength(250);

                entity.Property(e => e.Catigoria).HasMaxLength(500);

                entity.Property(e => e.DiakritosTitlos).HasMaxLength(500);

                entity.Property(e => e.Drastiriotita).HasMaxLength(500);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Epwnumia).HasMaxLength(500);

                entity.Property(e => e.Katastasi).HasMaxLength(500);

                entity.Property(e => e.Latitude).HasMaxLength(250);

                entity.Property(e => e.Longitude).HasMaxLength(250);
            });
        }
    }
}
