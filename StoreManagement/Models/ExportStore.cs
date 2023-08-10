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

    [StringLength(100)]
    public string ExporterName { get; set; } = null!;

    [StringLength(100)]
    public string Customer { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ExportDate { get; set; }

    [Column(TypeName = "money")]
    public float Total { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [InverseProperty("ExportStore")]
    public virtual ICollection<ExportStoreDetail> ExportStoreDetails { get; set; } = new List<ExportStoreDetail>();
}
