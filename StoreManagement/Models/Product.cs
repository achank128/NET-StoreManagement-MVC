using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Index("ProductCode", Name = "UQ__Products__2F4E024FEE43FB09", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string ProductCode { get; set; } = null!;

    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public Guid UnitId { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [Column(TypeName = "money")]
    public decimal? ImportPrice { get; set; }

    [Column(TypeName = "money")]
    public decimal Price { get; set; }

    public int Number { get; set; }

    public bool? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ExportStoreDetail> ExportStoreDetails { get; set; } = new List<ExportStoreDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ImportStoreDetail> ImportStoreDetails { get; set; } = new List<ImportStoreDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>();

    [ForeignKey("UnitId")]
    [InverseProperty("Products")]
    public virtual Unit Unit { get; set; } = null!;
}
