using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Index("Email", Name = "UQ__Users__A9D10534D1D8D039", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Role { get; set; }

    [InverseProperty("Exporter")]
    public virtual ICollection<ExportStore> ExportStores { get; set; } = new List<ExportStore>();

    [InverseProperty("Importer")]
    public virtual ICollection<ImportStore> ImportStores { get; set; } = new List<ImportStore>();

    [InverseProperty("Author")]
    public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>();
}
