using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ImportStoreDetail")]
public partial class ImportStoreDetail
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public Guid ImportStoreId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public decimal ImportPrice { get; set; }

    [ForeignKey("ImportStoreId")]
    [InverseProperty("ImportStoreDetails")]
    public virtual ImportStore ImportStore { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ImportStoreDetails")]
    public virtual Product Product { get; set; } = null!;
}
