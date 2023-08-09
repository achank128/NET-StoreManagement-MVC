using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ExportStoreDetail")]
public partial class ExportStoreDetail
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public Guid ExportStoreId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public float ExportPrice { get; set; }

    [ForeignKey("ExportStoreId")]
    [InverseProperty("ExportStoreDetails")]
    public virtual ExportStore ExportStore { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ExportStoreDetails")]
    public virtual Product Product { get; set; } = null!;
}
