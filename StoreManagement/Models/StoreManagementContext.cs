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

    public virtual DbSet<ExportStore> ExportStores { get; set; }

    public virtual DbSet<ImportStore> ImportStores { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=NKC\\SQLEXPRESS;Database=StoreManagement;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExportStore>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithMany(p => p.ExportStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportStore_Products");
        });

        modelBuilder.Entity<ImportStore>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithMany(p => p.ImportStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportStore_Products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
