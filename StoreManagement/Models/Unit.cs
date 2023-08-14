using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public partial class Unit
{
    [Key]
    [Column("ID")]
    [StringLength(20)]
    public string Id { get; set; } = null!;

    [StringLength(100)]
    public string UnitName { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [InverseProperty("Unit")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
