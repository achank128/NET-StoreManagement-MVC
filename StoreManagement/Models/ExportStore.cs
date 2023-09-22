using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ExportStore")]
public partial class ExportStore
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public Guid ExporterId { get; set; }

    public Guid CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExportDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? SubTotal { get; set; }

    [Column(TypeName = "money")]
    public decimal? Discount { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    [Column(TypeName = "money")]
    public decimal Total { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("ExportStores")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("ExportStore")]
    public virtual ICollection<ExportStoreDetail> ExportStoreDetails { get; set; } = new List<ExportStoreDetail>();

    [ForeignKey("ExporterId")]
    [InverseProperty("ExportStores")]
    public virtual User Exporter { get; set; } = null!;
}
