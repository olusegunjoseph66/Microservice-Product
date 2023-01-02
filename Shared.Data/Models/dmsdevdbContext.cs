using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Shared.Data.Models
{
    public partial class dmsdevdbContext : DbContext
    {
        public dmsdevdbContext()
        {
        }

        public dmsdevdbContext(DbContextOptions<dmsdevdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductStatus> ProductStatuses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", "Product");

                entity.HasIndex(e => new { e.Name, e.ProductType }, "IX_Products")
                    .IsUnique();

                entity.HasIndex(e => e.ProductStatusId, "IX_Products_Index");

                entity.Property(e => e.CompanyCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.ProductSapNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProductType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UnitOfMeasureCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProductStatus)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_ProductStatuses");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImages", "Product");

                entity.HasIndex(e => new { e.ProductId, e.CreatedByUserId, e.DateCreated }, "IX_ProductImages");

                entity.Property(e => e.CloudPath)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PublicUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductImages_Products");
            });

            modelBuilder.Entity<ProductStatus>(entity =>
            {
                entity.ToTable("ProductStatuses", "Product");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
