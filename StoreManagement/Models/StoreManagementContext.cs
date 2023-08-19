using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public partial class StoreManagementContext : DbContext
{
    public StoreManagementContext()
    {
    }

    public StoreManagementContext(DbContextOptions<StoreManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<ExportStore> ExportStores { get; set; }

    public virtual DbSet<ExportStoreDetail> ExportStoreDetails { get; set; }

    public virtual DbSet<ImportStore> ImportStores { get; set; }

    public virtual DbSet<ImportStoreDetail> ImportStoreDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPost> ProductPosts { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=NKC\\SQLEXPRESS;Database=StoreManagement;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<ExportStore>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.ExportStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportStore_Customer");

            entity.HasOne(d => d.Exporter).WithMany(p => p.ExportStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportStore_Exporter");
        });

        modelBuilder.Entity<ExportStoreDetail>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ExportStore).WithMany(p => p.ExportStoreDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportStoreDetail_ExportStore");

            entity.HasOne(d => d.Product).WithMany(p => p.ExportStoreDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportStoreDetail_Product");
        });

        modelBuilder.Entity<ImportStore>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Importer).WithMany(p => p.ImportStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportStore_Importer");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ImportStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportStore_Supplier");
        });

        modelBuilder.Entity<ImportStoreDetail>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ImportStore).WithMany(p => p.ImportStoreDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportStoreDetail_ImportStore");

            entity.HasOne(d => d.Product).WithMany(p => p.ImportStoreDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportStoreDetail_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products_Categories");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products_Unit");
        });

        modelBuilder.Entity<ProductPost>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Author).WithMany(p => p.ProductPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductPost_Author");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductPost_Product");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Role).HasDefaultValueSql("('NV')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
