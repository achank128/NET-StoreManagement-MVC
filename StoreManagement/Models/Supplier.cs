using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public partial class Supplier
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string SupplierName { get; set; } = null!;

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    public string? TaxCode { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [InverseProperty("Supplier")]
    public virtual ICollection<ImportStore> ImportStores { get; set; } = new List<ImportStore>();
}
