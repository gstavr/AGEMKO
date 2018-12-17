using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ConsoleAppAGEMKO.model
{
    public partial class mydbContext : DbContext
    {
        public mydbContext()
        {
        }

        public mydbContext(DbContextOptions<mydbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Businesses> Businesses { get; set; }
        public virtual DbSet<Individualcategory> Individualcategory { get; set; }
        public virtual DbSet<Mainactivity> Mainactivity { get; set; }
        public virtual DbSet<Municipality> Municipality { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Regionalunity> Regionalunity { get; set; }
        public virtual DbSet<Registrytype> Registrytype { get; set; }
        public virtual DbSet<Representative> Representative { get; set; }
        public virtual DbSet<Status> Status { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=127.0.0.1;uid=root;pwd=giorgos5756;database=mydb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Businesses>(entity =>
            {
                entity.ToTable("businesses", "mydb");

                entity.HasIndex(e => e.IndividualCategoryIndividualCategoryId)
                    .HasName("fk_Businesses_IndividualCategory1_idx");

                entity.HasIndex(e => e.MainActivityMainActivityId)
                    .HasName("fk_Businesses_MainActivity1_idx");

                entity.HasIndex(e => e.MunicipalityMunicipalityId)
                    .HasName("fk_Businesses_Municipality1_idx");

                entity.HasIndex(e => e.RegionRegionId)
                    .HasName("fk_Businesses_Region1_idx");

                entity.HasIndex(e => e.RegionalUnityRegionalUnityId)
                    .HasName("fk_Businesses_RegionalUnity1_idx");

                entity.HasIndex(e => e.RegistryTypeRegistryTypeId)
                    .HasName("fk_Businesses_RegistryType1_idx");

                entity.HasIndex(e => e.RepresentativeRepresentativeId)
                    .HasName("fk_Businesses_Representative1_idx");

                entity.HasIndex(e => e.StatusStatusId)
                    .HasName("fk_Businesses_Status1_idx");

                entity.Property(e => e.BusinessesId)
                    .HasColumnName("BusinessesID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.BusinessesAddress)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessesAgemko)
                    .HasColumnName("BusinessesAGEMKO")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessesAmke)
                    .HasColumnName("BusinessesAMKE")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessesDescr)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessesDistinctTitle)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessesEmail)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessesExtraField)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessesNumMembers).HasColumnType("int(11)");

                entity.Property(e => e.BusinessesReviewDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.BusinessesVat)
                    .HasColumnName("BusinessesVAT")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.IndividualCategoryIndividualCategoryId)
                    .HasColumnName("IndividualCategory_IndividualCategoryID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MainActivityMainActivityId)
                    .HasColumnName("MainActivity_MainActivityID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MunicipalityMunicipalityId)
                    .HasColumnName("Municipality_MunicipalityID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RegionRegionId)
                    .HasColumnName("Region_RegionID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RegionalUnityRegionalUnityId)
                    .HasColumnName("RegionalUnity_RegionalUnityID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RegistryTypeRegistryTypeId)
                    .HasColumnName("RegistryType_RegistryTypeID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RepresentativeRepresentativeId)
                    .HasColumnName("Representative_RepresentativeID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StatusStatusId)
                    .HasColumnName("Status_StatusID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IndividualCategoryIndividualCategory)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.IndividualCategoryIndividualCategoryId)
                    .HasConstraintName("fk_Businesses_IndividualCategory1");

                entity.HasOne(d => d.MainActivityMainActivity)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.MainActivityMainActivityId)
                    .HasConstraintName("fk_Businesses_MainActivity1");

                entity.HasOne(d => d.MunicipalityMunicipality)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.MunicipalityMunicipalityId)
                    .HasConstraintName("fk_Businesses_Municipality1");

                entity.HasOne(d => d.RegionRegion)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.RegionRegionId)
                    .HasConstraintName("fk_Businesses_Region1");

                entity.HasOne(d => d.RegionalUnityRegionalUnity)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.RegionalUnityRegionalUnityId)
                    .HasConstraintName("fk_Businesses_RegionalUnity1");

                entity.HasOne(d => d.RegistryTypeRegistryType)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.RegistryTypeRegistryTypeId)
                    .HasConstraintName("fk_Businesses_RegistryType1");

                entity.HasOne(d => d.RepresentativeRepresentative)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.RepresentativeRepresentativeId)
                    .HasConstraintName("fk_Businesses_Representative1");

                entity.HasOne(d => d.StatusStatus)
                    .WithMany(p => p.Businesses)
                    .HasForeignKey(d => d.StatusStatusId)
                    .HasConstraintName("fk_Businesses_Status1");
            });

            modelBuilder.Entity<Individualcategory>(entity =>
            {
                entity.ToTable("individualcategory", "mydb");

                entity.Property(e => e.IndividualCategoryId)
                    .HasColumnName("IndividualCategoryID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.IndividualCategoryDescr)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Mainactivity>(entity =>
            {
                entity.ToTable("mainactivity", "mydb");

                entity.Property(e => e.MainActivityId)
                    .HasColumnName("MainActivityID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.MainActivityDescr)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Municipality>(entity =>
            {
                entity.ToTable("municipality", "mydb");

                entity.HasIndex(e => e.RegionalUnityRegionalUnityId)
                    .HasName("fk_Municipality_RegionalUnity1_idx");

                entity.Property(e => e.MunicipalityId)
                    .HasColumnName("MunicipalityID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.MunicipalityDescr)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RegionalUnityRegionalUnityId)
                    .HasColumnName("RegionalUnity_RegionalUnityID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.RegionalUnityRegionalUnity)
                    .WithMany(p => p.Municipality)
                    .HasForeignKey(d => d.RegionalUnityRegionalUnityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Municipality_RegionalUnity1");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region", "mydb");

                entity.Property(e => e.RegionId)
                    .HasColumnName("RegionID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.RegionDescr)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Regionalunity>(entity =>
            {
                entity.ToTable("regionalunity", "mydb");

                entity.HasIndex(e => e.RegionRegionId)
                    .HasName("fk_RegionalUnity_Region1_idx");

                entity.Property(e => e.RegionalUnityId)
                    .HasColumnName("RegionalUnityID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.RegionRegionId)
                    .HasColumnName("Region_RegionID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RegionalUnityDescr)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.RegionRegion)
                    .WithMany(p => p.Regionalunity)
                    .HasForeignKey(d => d.RegionRegionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_RegionalUnity_Region1");
            });

            modelBuilder.Entity<Registrytype>(entity =>
            {
                entity.ToTable("registrytype", "mydb");

                entity.Property(e => e.RegistryTypeId)
                    .HasColumnName("RegistryTypeID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.RegistryTypeDescr)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Representative>(entity =>
            {
                entity.ToTable("representative", "mydb");

                entity.Property(e => e.RepresentativeId)
                    .HasColumnName("RepresentativeID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.RepresentativeFullName)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("status", "mydb");

                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.StatusDescr)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });
        }
    }
}
