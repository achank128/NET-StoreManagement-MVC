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

    [StringLength(100)]
    public string ImporterName { get; set; } = null!;

    [StringLength(100)]
    public string Supplier { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ImportDate { get; set; }

    [Column(TypeName = "money")]
    public float Total { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [InverseProperty("ImportStore")]
    public virtual ICollection<ImportStoreDetail> ImportStoreDetails { get; set; } = new List<ImportStoreDetail>();
}
