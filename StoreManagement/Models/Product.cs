using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public partial class Product
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ProductCode { get; set; } = null!;

    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    public Guid CategoryId { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Unit { get; set; }

    [Column(TypeName = "money")]
    public decimal? Price { get; set; }

    public int? Number { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public bool? Status { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ExportStoreDetail> ExportStoreDetails { get; set; } = new List<ExportStoreDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ImportStoreDetail> ImportStoreDetails { get; set; } = new List<ImportStoreDetail>();

    [ForeignKey("Unit")]
    [InverseProperty("Products")]
    public virtual Unit? UnitNavigation { get; set; }
}
