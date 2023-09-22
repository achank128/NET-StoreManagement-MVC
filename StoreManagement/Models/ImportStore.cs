using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ImportStore")]
public partial class ImportStore
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public Guid ImporterId { get; set; }

    public Guid SupplierId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ImportDate { get; set; }

    [Column(TypeName = "money")]
    public decimal Total { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [InverseProperty("ImportStore")]
    public virtual ICollection<ImportStoreDetail> ImportStoreDetails { get; set; } = new List<ImportStoreDetail>();

    [ForeignKey("ImporterId")]
    [InverseProperty("ImportStores")]
    public virtual User Importer { get; set; } = null!;

    [ForeignKey("SupplierId")]
    [InverseProperty("ImportStores")]
    public virtual Supplier Supplier { get; set; } = null!;
}
